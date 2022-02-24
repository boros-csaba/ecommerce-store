using BarionClientLibrary;
using BarionClientLibrary.Operations;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class BarionController : BaseController
    {
        private readonly BarionClient barionClient;
        private readonly IOrderService orderService;
        private readonly IEmailService emailService;
        private readonly IActionLogService actionLogService;
        private readonly ILogger<BarionController> logger;

        public BarionController(IConfiguration configuration, ICustomerService customerService, BarionClient barionClient, IEmailService emailService, IOrderService orderService, IActionLogService actionLogService, ILogger<BarionController> logger, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.barionClient = barionClient ?? throw new ArgumentNullException(nameof(barionClient));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public IActionResult StartPayment(bool newsletterConsent)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.StartBarionPayment, null, null);
            var order = orderService.GetCustomerActiveOrder(CustomerId);

            orderService.PlaceOrder(order.Id, newsletterConsent);
            Response.Cookies.Append("LastOrderId", order.OrderId, new CookieOptions { Expires = new DateTimeOffset(Helper.Now.AddHours(12)) });

            orderService.LogOrderAction(order.Id, OrderHistoryActionEnum.StartPayment, "Start Barion payment");
            var operationResult = StartPaymentOperation(order);
            if (operationResult.IsOperationSuccessful)
            {
                var startPaymentReult = operationResult as StartPaymentOperationResult;
                return Redirect(startPaymentReult.GatewayUrl);
            }
            else
            {
                var errorMessage = new StringBuilder("Barion StartPayment Error");
                foreach (var error in operationResult.Errors)
                {
                    errorMessage.AppendLine("Error details:");
                    errorMessage.AppendLine(error.Title);
                    errorMessage.AppendLine(error.ErrorCode);
                    errorMessage.AppendLine(error.Description);
                    errorMessage.AppendLine(error.ToString());
                }
                errorMessage.AppendLine(operationResult.ToString());
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Barion error", errorMessage.ToString(), errorMessage.ToString(), "error");
            }

            return Ok();
        }

        [Route("/rendeles/kartyas-fizetes/{customerId}-{orderId}")]
        public IActionResult StartPayment(int customerId, int orderId, string s)
        {
            var order = orderService.GetOrderById(orderId);
            if (order == null || order.CustomerId != customerId)
            {
                return Ok();
            }

            orderService.ChangePaymentMethod(order.OrderId, order.Customer.CookieId, PaymentMethodEnum.Barion);

            orderService.LogOrderAction(order.Id, OrderHistoryActionEnum.StartPayment, "Start Barion payment" + (s == null ? "" : " " + s));
            var operationResult = StartPaymentOperation(order);

            if (operationResult.IsOperationSuccessful)
            {
                var startPaymentReult = operationResult as StartPaymentOperationResult;
                return Redirect(startPaymentReult.GatewayUrl);
            }
            else
            {
                var errorMessage = new StringBuilder("Barion StartPayment Error");
                foreach (var error in operationResult.Errors)
                {
                    errorMessage.AppendLine("Error details:");
                    errorMessage.AppendLine(error.Title);
                    errorMessage.AppendLine(error.ErrorCode);
                    errorMessage.AppendLine(error.Description);
                    errorMessage.AppendLine(error.ToString());
                }
                errorMessage.AppendLine(operationResult.ToString());
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Barion error", errorMessage.ToString(), errorMessage.ToString(), "error");
            }

            return Ok();
        }

        private BarionOperationResult StartPaymentOperation(Order order)
        {
            var items = order.OrderItems.Select(oi => new Item
            {
                Name = oi.Name,
                Description = string.IsNullOrWhiteSpace(oi.Name) ? "Karkötő" : oi.Name,
                Quantity = oi.Quantity,
                Unit = "darab",
                UnitPrice = oi.UnitPrice,
                ItemTotal = oi.Quantity * oi.UnitPrice,
                SKU = (oi as BraceletOrderItem)?.Product?.IdString ?? "bracelt-designer"
            }).ToArray();

            var payment = orderService.CreateNewPayment(order);
            var paymentTransaction = new PaymentTransaction
            {
                POSTransactionId = payment.TransactionId,
                Payee = "info@elenora.hu",
                Total = order.Total,
                Items = items,
                Comment = order.Remark
            };

            var startPaymentOperation = new StartPaymentOperation
            {
                PaymentType = PaymentType.Immediate,
                GuestCheckOut = true,
                FundingSources = new[] { FundingSourceType.All },
                PayerHint = order.EmailAddress?.Address,
                CardHolderNameHint = order.BillingAddress.Name,
                Locale = CultureInfo.GetCultureInfo("hu-HU"),
                Currency = Currency.HUF,
                Transactions = new[] { paymentTransaction },
#if E2E
                CallbackUrl = "https://192.168.0.105:45455/Barion/Callback",
                RedirectUrl = "https://192.168.0.105:45455/rendeles/" + payment.TransactionId
#else
                CallbackUrl = UriHelper.BuildAbsolute("https", Request.Host, Url.Action("Callback", "Barion")),
                RedirectUrl = UriHelper.BuildAbsolute("https", Request.Host, Url.Action("OrderConfirmation", "Checkout", new { id = payment.TransactionId }))
#endif
            };

            var task = barionClient.ExecuteAsync(startPaymentOperation);
            task.Wait();
            return task.Result;
        }

        public IActionResult Callback(Guid PaymentId)
        {
            var getPaymentState = new GetPaymentStateOperation
            {
                PaymentId = PaymentId
            };

            var task = barionClient.ExecuteAsync(getPaymentState);
            task.Wait();
            var result = task.Result;

            if (result.IsOperationSuccessful)
            {
                var paymentState = result as GetPaymentStateOperationResult;
                var transactionId = paymentState.Transactions.First().POSTransactionId;
                var payment = orderService.GetPaymentByTransactionId(transactionId);
                orderService.UpdatePayment(payment, paymentState.PaymentId.ToString(), paymentState.Transactions.First().TransactionId.ToString(), paymentState.FraudRiskScore, paymentState.Status.ToString());
                return Json(new { Success = true, paymentState.Status });
            }
            else
            {
                var stringBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    stringBuilder.AppendLine(error.Title);
                    stringBuilder.AppendLine(error.ErrorCode);
                    stringBuilder.AppendLine(error.Description);
                }
                logger.Log(LogLevel.Error, stringBuilder.ToString());
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Barion callback error", stringBuilder.ToString(), stringBuilder.ToString(), "error");
            }

            return Json(new { Success = false });
        }

        [Route("/rendeles/status/{transactionId}")]
        public IActionResult GetPaymentStatus(string transactionId)
        {
            var status = orderService.GetPaymentByTransactionId(transactionId).Status;
            return Json(status);
        }

    }
}
