using elenora.Features.ProductPricing;
using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace elenora.Features.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly IProductPricingService productPricingService;
        private readonly DataContext context;

        public CartRepository(IProductPricingService productPricingService, DataContext context)
        {
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Models.Cart GetCustomerCart(int customerId)
        {
            var cart = context.Carts
                .Include(c => c.Coupon)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as BraceletCartItem).Product).ThenInclude(p => p.Category)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as BraceletCartItem).Product).ThenInclude(p => p.ProductDiscounts)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomBraceletCartItem).Components)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomBraceletCartItem).BeadType)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomBraceletCartItem).SecondaryBeadType)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomTextBraceletCartItem).Product).ThenInclude(p => p.Category)
                .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomTextBraceletCartItem).Product).ThenInclude(p => p.ProductDiscounts)
                .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemComplementaryProducts).ThenInclude(cip => cip.ComplementaryProduct)
                .FirstOrDefault(c => c.CustomerId == customerId);
            if (cart != null)
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var bracelet = (cartItem as BraceletCartItem)?.Product ?? (cartItem as CustomTextBraceletCartItem)?.Product;
                    if (bracelet != null) bracelet.Price = productPricingService.GetProductPrice(bracelet);
                }
            }
            return cart;
        }
    }
}
