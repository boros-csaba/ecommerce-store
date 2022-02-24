using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewComponents
{
    public class CartViewComponent: ViewComponent
    {
        private readonly ICartService cartService;
        private readonly IPromotionService promotionService;

        public CartViewComponent(ICartService cartService, IPromotionService promotionService)
        {
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        public IViewComponentResult Invoke(int customerId)
        {
            var cart = cartService.GetCustomerCart(customerId);
            var model = new CartDrawerViewModel
            {
                Cart = new CartViewModel(cart, cartService),
                Promotions = promotionService.GetActivePromotions()
            };
            model.Cart.Promotions = model.Promotions;
            return View(model);
        }
    }
}
