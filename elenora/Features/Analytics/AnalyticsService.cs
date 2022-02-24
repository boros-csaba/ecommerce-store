using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Features.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private const string pixelId = "1023971627979082"; //todo, configbol
        private const string accessToken = "EAAEnUrp3MHsBABwDxebyOFb51FsVu42PdfMJANEpWnTpV78ECYogiz5UTCCJAjDf7aC5yNapQ5pUGks0pZBoOI3xwISHSZAO5ezDiVky42P0Madp2qlGwFD7I7iI2f5B2VKQoZCd1SdiP1j6KHpQsBaSbuHHvNlDFcnj7RanMK64nZBSL0B3";
        private readonly string apiUrl = $"https://graph.facebook.com/v11.0/{pixelId}/events";

        private readonly IEmailService emailService;
        private readonly ICustomerService customerService;
        private readonly DataContext context;
        private readonly IProductService productService;
        private readonly IConfiguration configuration;

        public AnalyticsService(IEmailService emailService, ICustomerService customerService, DataContext context, IProductService productService, IConfiguration configuration)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string ReportProductView(Bracelet product, HttpRequest httpRequest, string cookieId)
        {
            try
            {
                if (!configuration.GetValue<bool>("Settings:AddFbPixel")) return null;
                
                var eventId = Guid.NewGuid().ToString();
                var customData = GetCustomData(product);
                var data = GetData("ViewContent", httpRequest, eventId, customData, cookieId);

                SendInformation(data);
                return eventId;
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            return null;
        }

        public void ReportAddToCart(int productId, int quantity, HttpRequest httpRequest, string cookieId, string eventId)
        {
            try
            {
                if (!configuration.GetValue<bool>("Settings:AddFbPixel")) return;

                var customData = GetCustomData(productService.GetBracelet(productId), quantity);
                var data = GetData("AddToCart", httpRequest, eventId, customData, cookieId);

                SendInformation(data);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        private void HandleException(Exception e)
        {
            var message = e.ToString() + e.StackTrace + e.InnerException.ToString();
            emailService.SendEmailToAdmins("Fb Pixel Error", message, message, "pixel-error");
        }

        private void SendInformation(string data)
        {
            using var formData = new MultipartFormDataContent
                {
                    { new StringContent(accessToken), "access_token" },
                    { new StringContent(data, Encoding.UTF8, "application/json"), "data" }
                };
            //formData.Add(new StringContent("TEST43194"), "test_event_code");

            var result = new HttpClient().PostAsync(apiUrl, formData).Result;
            if (!result.IsSuccessStatusCode)
            {
                emailService.SendEmailToAdmins("Fb Pixel Error", result.ReasonPhrase, result.ReasonPhrase, "pixel-error");
            }
        }

        private List<string> GetCustomData(Bracelet product, int quantity = 1)
        {
            var customData = new List<string>();
            customData.Add($@"""content_name"":""{product.Name}""");
            customData.Add($@"""content_category"":""{product.Category.Name}""");
            customData.Add($@"""content_type"":""product""");
            customData.Add($@"""content_ids"":[""{product.IdString}""]");
            customData.Add($@"""contents"":[{{""id"":""{product.IdString}"",""quantity"":{quantity},""item_price"":{product.Price.Price}}}]");
            return customData;
        }

        private string GetData(string eventName, HttpRequest httpRequest, string eventId, List<string> additionalCustomData, string cookieId)
        {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var customer = customerService.GetOrCreateCustomer(cookieId);

            var result = new StringBuilder();
            result.Append("[{");
            result.Append($@"""event_name"":""{eventName}"",");
            result.Append($@"""event_time"":{unixTimestamp},");
            result.Append($@"""action_source"":""website"",");
            result.Append($@"""event_id"":""{eventId}"",");

            var requestUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.Path}{httpRequest.QueryString}";
            result.Append($@"""event_source_url"":""{requestUrl}"",");

            result.Append(@"""user_data"":{");
            var userData = new List<string>();
            if (!string.IsNullOrWhiteSpace(customer.EmailAddress?.Address))
            {
                var email = GetHashedValue(customer.EmailAddress.Address.ToLower().Trim());
                userData.Add($@"""em"":""{email}""");
            }
            var customerOrder = context.Orders
                .OrderByDescending(o => o.CreatedDate)
                .Include(o => o.BillingAddress).ToList()
                .Where(o => o.CustomerId == customer.Id && o.BillingAddress != null && o.BillingAddress.Name != null)
                .FirstOrDefault();
            if (customerOrder != null && customerOrder.BillingAddress != null)
            {
                var name = customerOrder.BillingAddress.Name;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var firstName = name.Split(' ').First().Trim();
                    if (!string.IsNullOrWhiteSpace(firstName))
                    {
                        userData.Add($@"""fn"":""{GetHashedValue(firstName.ToLower())}""");
                    }
                    var lastName = name.Split(' ').Last().Trim();
                    if (!string.IsNullOrWhiteSpace(lastName))
                    {
                        userData.Add($@"""ln"":""{GetHashedValue(lastName.ToLower())}""");
                    }
                }

                if (!string.IsNullOrWhiteSpace(customerOrder.Phone))
                {
                    var phone = GetHashedValue($"36{customerOrder.Phone}");
                    userData.Add($@"""ph"":""{phone}""");
                }

                if (!string.IsNullOrWhiteSpace(customerOrder.BillingAddress.City))
                {
                    userData.Add($@"""ct"":""{GetHashedValue(customerOrder.BillingAddress.City.ToLower().Replace(" ", ""))}""");
                }

                if (!string.IsNullOrWhiteSpace(customerOrder.BillingAddress.ZipCode))
                {
                    userData.Add($@"""zp"":""{GetHashedValue(customerOrder.BillingAddress.ZipCode)}""");
                }

                userData.Add($@"""country"":""{GetHashedValue("hu")}""");
            }

            userData.Add($@"""external_id"":""{cookieId}""");

            var ipAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                userData.Add($@"""client_ip_address"":""{ipAddress}""");
            }

            if (httpRequest.Headers.ContainsKey("User-Agent"))
            {
                var browser = httpRequest.Headers["User-Agent"].ToString();
                userData.Add($@"""client_user_agent"":""{browser}""");
            }

            if (httpRequest.Cookies.ContainsKey("_fbp"))
            {
                userData.Add($@"""fbp"":""{httpRequest.Cookies["_fbp"]}""");
            }

            if (httpRequest.Cookies.ContainsKey("_fbc"))
            {
                userData.Add($@"""fbc"":""{httpRequest.Cookies["_fbc"]}""");
            }

            var isFirst = true;
            foreach (var item in userData)
            {
                if (isFirst) isFirst = false;
                else result.Append(",");
                result.Append(item);
            }
            result.Append("},");

            result.Append(@"""custom_data"":{");
            var customData = new List<string>(additionalCustomData);
            customData.Add($@"""currency"":""HUF""");
            isFirst = true;
            foreach (var item in customData)
            {
                if (isFirst) isFirst = false;
                else result.Append(",");
                result.Append(item);
            }
            result.Append("}");

            result.Append("}]");

            return result.ToString();
        }

        private string GetHashedValue(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using var hashEngine = SHA256.Create();
            var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
            var sb = new StringBuilder();
            foreach (var b in hashedBytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
