using BarionClientLibrary.Operations.Common;
using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using elenora.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel(Cart cart, ICartService cartService)
        {
            CustomerId = cart.CustomerId;
            foreach (var cartItem in cart.CartItems)
            {
                var itemModel = new CartItemViewModel
                {
                    Id = cartItem.Id,
                    ProductName = cartItem.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.ItemPrice,
                    OriginalUnitPrice = cartItem.ItemOriginalPrice,
                    Timestamp = Helper.Now.Ticks.ToString()
                };
                if (cartItem is IBraceletWithSize)
                {
                    itemModel.BraceletSize = (cartItem as IBraceletWithSize).BraceletSize;
                }
                if (cartItem is CustomBraceletCartItem)
                {
                    itemModel.ProductId = 100000;
                    itemModel.IsCustomBracelet = true;
                    itemModel.ProductType = ProductTypeEnum.Bracelet;
                    itemModel.ProductIdString = "custom-bracelet";
                    itemModel.ProductCategory = "Egyedi karkötő";
                    itemModel.Variant = $"{(cartItem as CustomBraceletCartItem).StyleType} - {(cartItem as CustomBraceletCartItem).BeadType.Name}";
                    if ((cartItem as CustomBraceletCartItem).SecondaryBeadType != null)
                    {
                        itemModel.Variant += $" - {(cartItem as CustomBraceletCartItem).SecondaryBeadType.Name}"; 
                    }
                }
                else if (cartItem is BraceletCartItem)
                {
                    var braceletCartItem = cartItem as BraceletCartItem;
                    itemModel.ProductId = braceletCartItem.ProductId;
                    itemModel.IsCustomBracelet = false;
                    itemModel.BraceletSize2 = braceletCartItem.BraceletSize2;
                    itemModel.ProductIdString = braceletCartItem.Product.IdString;
                    itemModel.ProductImgageUrl = braceletCartItem.Product.MainImage;
                    itemModel.ProductType = braceletCartItem.Product.ProductType;
                    itemModel.ProductCategory = braceletCartItem.Product.Category.Name;
                }
                else if (cartItem is CustomTextBraceletCartItem)
                {
                    var braceletCartItem = cartItem as CustomTextBraceletCartItem;
                    itemModel.ProductId = braceletCartItem.ProductId;
                    itemModel.IsCustomBracelet = false;
                    itemModel.ProductIdString = braceletCartItem.Product.IdString;
                    itemModel.ProductImgageUrl = braceletCartItem.Product.MainImage;
                    itemModel.ProductType = braceletCartItem.Product.ProductType;
                    itemModel.ProductCategory = braceletCartItem.Product.Category.Name;
                }
                else if (cartItem is StringBraceletCartItem)
                {
                    itemModel.ProductId = 200000;
                    itemModel.IsStringBracelet = true;
                    itemModel.ProductType = ProductTypeEnum.SingleSizeBracelet;
                    itemModel.ProductIdString = "string-bracelet";
                    itemModel.ProductCategory = "Fonott karkötő";
                    itemModel.StringBracelet = new StringBraceletViewModel(cartItem as StringBraceletCartItem);
                }
                foreach (var complementaryProduct in cartItem.CartItemComplementaryProducts)
                {
                    var complementaryProductModel = new ComplementaryProductCartItemViewModel
                    {
                        Id = complementaryProduct.Id,
                        Name = complementaryProduct.ComplementaryProduct.Name,
                        Price = complementaryProduct.ComplementaryProduct.Price,
                        ImageUrl = complementaryProduct.ComplementaryProduct.ImageUrl
                    };
                    itemModel.ComplementaryProducts.Add(complementaryProductModel);
                }
                itemModel.UnitPrice += itemModel.ComplementaryProducts.Sum(p => p.Price);
                CartItems.Add(itemModel);
            }

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
        public int CustomerId { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public bool HasCoupon { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public string CouponDescription { get; set; }
        public int? CouponPercentage { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
        public List<Promotion> Promotions { get; set; }
    }
}
