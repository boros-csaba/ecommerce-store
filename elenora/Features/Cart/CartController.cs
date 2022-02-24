using elenora.Features.Analytics;
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
    public class CartController : BaseController
    {
        private readonly ICartService cartService;
        private readonly IActionLogService actionLogService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly IAnalyticsService analyticsService;

        public CartController(IProductService productService, IConfiguration configuration, ICustomerService customerService, ICartService cartService, IActionLogService actionLogService, IPromotionService promotionService, IAnalyticsService analyticsService) : base(configuration, customerService, promotionService)
        {
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));

        }

        [HttpPost]
        [Route("/kosar/hozzaadas")]
        public IActionResult AddToCart(int productId, int quantity, BraceletSizeEnum? braceletSize, BraceletSizeEnum? braceletSize2, string customText, string eventId)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.AddToCart, productId, $"Quantity: {quantity}");
            productService.LogProductAddToCart(productId);
            analyticsService.ReportAddToCart(productId, quantity, Request, CookieId, eventId);
            cartService.AddToCart(CustomerId, productId, quantity, braceletSize, braceletSize2, customText);
            return ViewComponent("Cart", CustomerId);
        }

        [HttpPost]
        [Route("kosar/kupon")]
        public IActionResult AddCoupon(string code)
        {
            if (code == null) return Json(string.Empty);
            var result = cartService.ApplyCoupon(CustomerId, code);
            if (!result.Success) return Json(new CartModel(result.ErrorMessage));
            var cart = cartService.GetCustomerCart(CustomerId);
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.AddCoupon, null, code);
            return Json(new CartModel(cart, cartService));
        }

        [HttpPost]
        [Route("/kosar/kupon-torles")]
        public IActionResult RemoveCouponFromCart()
        {
            cartService.RemoveCouponFromCart(CustomerId);
            var cart = cartService.GetCustomerCart(CustomerId);
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.DeleteCoupon, null, null);
            return Json(new CartModel(cart, cartService));
        }

        [HttpPost]
        [Route("/kosar/mennyiseg")]
        public IActionResult ChangeItemQuantity(int cartItemId, int quantity)
        {
            cartService.ChangeItemQuantity(CustomerId, cartItemId, quantity);
            var cart = cartService.GetCustomerCart(CustomerId);
            return Json(new CartModel(cart, cartService));
        }

        [HttpPost]
        [Route("/kosar/meret")]
        public IActionResult ChangeItemSize(int cartItemId, BraceletSizeEnum size, bool isSize2)
        {
            cartService.ChangeItemSize(CustomerId, cartItemId, size, isSize2);
            var cart = cartService.GetCustomerCart(CustomerId);
            return Json(new CartModel(cart, cartService));
        }

        [HttpPost]
        [Route("/kosar/torles")]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            cartService.RemoveFromCart(CustomerId, cartItemId);
            var cart = cartService.GetCustomerCart(CustomerId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.DeleteFromCart, (cartItem as BraceletCartItem)?.ProductId, null);
            return Json(new CartModel(cart, cartService));
        }

        [HttpGet]
        [Route("/kosar/folytatas/{newUserGuid}")]
        [Route("/kosar/folytatas/{newUserGuid}/{coupon}")]
        public IActionResult MergeCartContents(string newUserGuid, string coupon = null)
        {
            var customer = customerService.GetOrCreateCustomer(newUserGuid);
            actionLogService.LogAction(customer.Id, UserHelper.GetActionLogInformation(Request), ActionEnum.ContinueShoppingFromEmail, null, null);
            var cookieOptions = new CookieOptions
            {
                Expires = new DateTimeOffset(Helper.Now.AddYears(10))
            };
            Response.Cookies.Append("Id", newUserGuid, cookieOptions);
            if (coupon != null)
            {
                var newCustomer = customerService.GetOrCreateCustomer(newUserGuid);
                cartService.ApplyCoupon(newCustomer.Id, coupon);
            }
            return RedirectToAction("Cart", "Checkout");
        }
    }
}
