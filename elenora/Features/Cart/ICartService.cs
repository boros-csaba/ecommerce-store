using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface ICartService
    {
        Cart GetCustomerCart(int customerId);
        void AddToCart(int customerId, int productId, int quantity, BraceletSizeEnum? braceletSize, BraceletSizeEnum? braceletSize2, string customText);
        void AddToCart(int customerId, int cartItemId, int beadTypeId, int secondaryBeadTypeId, CustomBraceletStyleEnum styleType, int[] componentIds, BraceletSizeEnum braceletSize, int[] complementaryProducts);
        void AddToCart(int customerId, BraceletTypeEnum braceletType, string knotColor, string string1Color, string string2Color, string string3Color, string flap1Color, string flap2Color);
        void ChangeItemQuantity(int customerId, int cartItemId, int quantity);
        void ChangeItemSize(int customerId, int cartItemId, BraceletSizeEnum braceletSize, bool isSize2);
        void RemoveFromCart(int customerId, int cartItemId);
        Result ApplyCoupon(int customerId, string couponCode);
        void RemoveCouponFromCart(int customerId);
        decimal GetCartTotal(int customerId);
        decimal GetCartTotal(Cart cart);
        decimal GetShippingPrice(int customerId);
        decimal GetShippingPrice(Cart cart);
        decimal GetShippingPrice(decimal total, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod);
        decimal GetCouponValue(int customer);
        decimal GetCouponValue(Cart cart);
    }
}
