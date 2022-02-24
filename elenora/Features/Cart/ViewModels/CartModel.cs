using elenora.Models;
using elenora.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CartModel
    {
        public CartModel(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public CartModel(Cart cart, ICartService cartService)
        {
            Success = true;
            CartItems = new ReadOnlyCollection<CartItemModel>(cart.CartItems.Select(i => new CartItemModel(i)).ToList());
            HasCoupon = cart.Coupon != null;
            if (HasCoupon)
            {
                var couponValue = cartService.GetCouponValue(cart);
                CouponName = cart.Coupon.Name;
                CouponCode = cart.Coupon.Code.ToUpper();
                CouponDescription = cart.GetCouponWarning(couponValue);
                CouponAmount = couponValue;
                CouponPercentage = cart.Coupon.Percentage;
            }
            ShippingPrice = cartService.GetShippingPrice(cart);
            Total = cartService.GetCartTotal(cart);
        }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public ReadOnlyCollection<CartItemModel> CartItems { get; set; }
        public bool HasCoupon { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public string CouponDescription { get; set; }
        public int? CouponPercentage { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
    }
}
