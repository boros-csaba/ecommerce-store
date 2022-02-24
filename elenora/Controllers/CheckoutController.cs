using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly ICartService cartService;
        private readonly IActionLogService actionLogService;
        private readonly IPromotionService promotionService;
        private readonly ICustomerService customerService;

        public CheckoutController(IConfiguration configuration, ICustomerService customerService, IOrderService orderService, ICartService cartService, IActionLogService actionLogService, IPromotionService promotionService): base(configuration, customerService, promotionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [HttpGet]
        [Route("/rendeles/kosar")]
        public IActionResult Cart()
        {
            var model = GetCheckoutModel();
            return View(model);
        }

        [HttpGet]
        [Route("/rendeles/szallitasi-mod")]
        public IActionResult ShippingMethod()
        {
            var model = GetCheckoutModel();
            if (!model.Cart.CartItems.Any()) return RedirectToAction("Cart");
            return View(model);
        }

        [HttpPost]
        [Route("/rendeles/szallitasi-mod")]
        public IActionResult ShippingMethod(ShippingMethodSaveModel data)
        {
            var result = orderService.CompleteShippingMethodStep(CustomerId, data.Validate, data.EmailAddress, (ShippingMethodEnum)data.ShippingMethod, data.ShippingPointAddressInformation);
            return Ok(result);
        }

        [HttpGet]
        [Route("/rendeles/szamlazasi-adatok")]
        public IActionResult BillingInformation()
        {
            var model = GetCheckoutModel();
            if (!model.Cart.CartItems.Any()) return RedirectToAction("Cart");
            return View(model);
        }

        [HttpPost]
        [Route("/rendeles/szamlazasi-adatok")]
        public IActionResult BillingInformation(BillingInformationSaveModel data)
        {
            var result = orderService.CompleteBillingInformationStep(CustomerId, data.Validate, data.BillingName, data.BillingZip, data.BillingCity, data.BillingAddress, data.Phone, data.Remark, (PaymentMethodEnum)data.PaymentMethod, data.DifferentShippingAddress, data.ShippingName, data.ShippingZip, data.ShippingCity, data.ShippingAddress);
            return Ok(result);
        }

        [HttpGet]
        [Route("/rendeles/osszegzes")]
        public IActionResult OrderSummary()
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewPage, null, "/rendeles/osszegzes");
            var model = GetCheckoutModel();
            if ((OrderStatusEnum)model.OrderStatus == OrderStatusEnum.NewOrder)
            {
                var lastOrderId = Request.Cookies["LastOrderId"];
                if (!string.IsNullOrWhiteSpace(lastOrderId))
                {
                    var customer = customerService.GetCustomerById(CustomerId);
                    return RedirectToAction("OrderConfirmation", "Checkout", new { id = $"{lastOrderId}-{customer.CookieId}", e = true });
                }
            }
            if (!model.Cart.CartItems.Any()) return RedirectToAction("Cart");
            return View(model);
        }

        private CheckoutModel GetCheckoutModel()
        {
            var cart = cartService.GetCustomerCart(CustomerId);
            var order = orderService.GetCustomerActiveOrder(CustomerId);

            var model = new CheckoutModel
            {
                OrderId = order.Id,
                CustomerId = CustomerId,
                Cart = new CartViewModel(cart, cartService),
                OrderStatus = (int)order.Status,
                Remark = order.Remark,
                Email = order.EmailAddress?.Address,
                Phone = order.Phone,
                ShippingMethod = order.ShippingMethod,
                DifferentShippingAddress = order.DifferentShippingAddress,
                PaymentMethod = order.PaymentMethod,
                BillingName = order.BillingAddress.Name,
                BillingZip = order.BillingAddress.ZipCode,
                BillingCity = order.BillingAddress.City,
                BillingAddress = order.BillingAddress.AddressLine,
                Total = cartService.GetCartTotal(CustomerId),
                Promotions = promotionService.GetActivePromotions(),
                ShippingPointAddressInformation = order.ShippingPointAddressInformation
            };
            model.Cart.Promotions = model.Promotions;
            if (order.ShippingAddress != null)
            {
                model.ShippingName = order.ShippingAddress.Name;
                model.ShippingZip = order.ShippingAddress.ZipCode;
                model.ShippingCity = order.ShippingAddress.City;
                model.ShippingAddress = order.ShippingAddress.AddressLine;
            }

            return model;
        }

        [HttpPost]
        [Route("/rendeles/rendeles-elkuldese")]
        public IActionResult PlaceOrder(bool newsletterConsent)
        {
            var order = orderService.GetCustomerActiveOrder(CustomerId);
            orderService.PlaceOrder(order.Id, newsletterConsent);
            Response.Cookies.Append("LastOrderId", order.OrderId, new CookieOptions { Expires = new DateTimeOffset(Helper.Now.AddHours(12)) });
            var customer = customerService.GetCustomerById(CustomerId);

            return Ok(new { url = Url.Action("OrderConfirmation", new { id = $"{order.OrderId}-{customer.CookieId}" }) });
        }

        [HttpGet]
        [Route("/rendeles/{id}")]
        public IActionResult OrderConfirmation(string id, bool e = false)
        {
            string orderId, customerId;
            var idParts = id.Split("-");
            if (idParts.Length == 7)
            {
                orderId = $"{idParts[0]}-{idParts[1]}";
                customerId = $"{idParts[2]}-{idParts[3]}-{idParts[4]}-{idParts[5]}-{idParts[6]}";
            }
            else
            {
                var transaction = orderService.GetPaymentByTransactionId(id);
                orderId = transaction.Order.OrderId;
                customerId = transaction.Order.Customer.CookieId;
            }

            var order = orderService.GetCustomerOrder(orderId, customerId);
            if (order == null) throw new ArgumentException($"Order cannot be found with Id '{id}'");

            var transactionId = "";
            var transactionStatus = "";
            if (order.Payments.Any())
            {
                var lastPayment = order.Payments.OrderByDescending(p => p.CreatedDate).First();
                transactionId = lastPayment.TransactionId;
                transactionStatus = lastPayment.Status;
                if (e && transactionStatus != "Succeeded")
                {
                    transactionStatus = "Error";
                }
            }

            return View(new OrderConfirmationViewModel(order)
            {
                TransactionId = transactionId,
                TransactionStatus = transactionStatus,
                CustomerIdString = order.Customer.CookieId
            });
        }

        [Route("/rendeles/fizetesi-mod/{id}")]
        public IActionResult ChangePaymentMethod(string id, PaymentMethodEnum pm)
        {
            var idParts = id.Split("-");
            var orderId = $"{idParts[0]}-{idParts[1]}";
            var customerId = $"{idParts[2]}-{idParts[3]}-{idParts[4]}-{idParts[5]}-{idParts[6]}";
            orderService.ChangePaymentMethod(orderId, customerId, pm);
            return RedirectToAction("OrderConfirmation", "Checkout", new { id = id });
        }
    }
}
