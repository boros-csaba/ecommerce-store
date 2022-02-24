using elenora.Features.Cart;
using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace elenora.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext context;
        private readonly ICartRepository repository;
        private readonly IProductService productService;
        private readonly IPromotionService promotionService;

        public CartService(IProductService productService, DataContext context, ICartRepository repository, IPromotionService promotionService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        public Cart GetCustomerCart(int customerId)
        {
            var cart = repository.GetCustomerCart(customerId);
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedDate = Helper.Now,
                    ShippingMethod = ShippingMethodEnum.GLS,
                    PaymentMethod = PaymentMethodEnum.Barion,
                    CartItems = new List<CartItem>()
                };
                context.Carts.Add(cart);
                context.SaveChanges();
                var customer = context.Customers.First(c => c.Id == customerId);
                customer.CartId = cart.Id;
                context.SaveChanges();
            }
            return cart;
        }

        public decimal GetCartTotal(int customerId)
        {
            var cart = repository.GetCustomerCart(customerId);
            return GetCartTotal(cart);
        }

        
        public decimal GetCartTotal(Cart cart)
        {
            var total = GetCartTotalWithoutShipping(cart);
            var shippingPrice = GetShippingPrice(total, cart.ShippingMethod, cart.PaymentMethod);
            return total + shippingPrice;
        }

        public decimal GetShippingPrice(int customerId)
        {
            var cart = GetCustomerCart(customerId);
            return GetShippingPrice(cart);
        }

        public decimal GetShippingPrice(Cart cart)
        {
            var total = GetCartTotalWithoutShipping(cart);
            return GetShippingPrice(total, cart.ShippingMethod, cart.PaymentMethod);
        }

        public decimal GetShippingPrice(decimal total, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod)
        {
            if (total >= Settings.FREE_SHIPPING_THRESHOLD)
            {
                if (Settings.FREE_SHIPPING_INCLUDES_PAYMENT || paymentMethod != PaymentMethodEnum.PayAtDelivery)
                    return 0;
                return Settings.GLS_PAYMENT_PRICE;
            }

            decimal price = 0;
            if (shippingMethod == ShippingMethodEnum.GLS)
            {
                price += Settings.GLS_SHIPPING_PRICE(promotionService.IsPromotionActive(PromotionEnum.FreeShipping));
            }
            else
            {
                price += Settings.GLS_CSOMAGPONT_SHIPPING_PRICE(promotionService.IsPromotionActive(PromotionEnum.FreeShipping));
            }
            if (paymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                price += Settings.GLS_PAYMENT_PRICE;
            }
            return price;
        }

        public decimal GetCouponValue(int customerId)
        {
            var cart = GetCustomerCart(customerId);
            return GetCouponValue(cart);
        }

        public decimal GetCouponValue(Cart cart)
        {
            if (cart.Coupon == null) return 0;
            decimal value = 0;
            int couponPercentage = cart.Coupon?.Percentage ?? 0;
            decimal total = 0;
            decimal lowestPrice = int.MaxValue;

            foreach (var item in cart.CartItems)
            {
                var itemPrice = item.ItemPrice + item.CartItemComplementaryProducts.Sum(ci => ci.ComplementaryProduct.Price);
                var discounted = item.ItemOriginalPrice.HasValue;
                if (!discounted && couponPercentage > 0)
                {
                    value -= Math.Round(itemPrice * item.Quantity * couponPercentage / 100);
                }
                if (itemPrice < lowestPrice)
                {
                    lowestPrice = itemPrice;
                }
                total += itemPrice * item.Quantity;
            }

            if (cart.Coupon.Percentage != null) return value;
            if (cart.Coupon.Value != null && (cart.Coupon.MinCartValue == null || total >= cart.Coupon.MinCartValue.Value))
            {
                return -cart.Coupon.Value.Value;
            }
            if (cart.Coupon.GetOneFreeMinimumQuantity > 0 && cart.CartItems.Sum(ci => ci.Quantity) > cart.Coupon.GetOneFreeMinimumQuantity)
            {
                return -lowestPrice;
            }
            return 0;
        }

        public void AddToCart(int customerId, int productId, int quantity, BraceletSizeEnum? braceletSize, BraceletSizeEnum? braceletSize2, string customText)
        {
            var cart = GetCustomerCart(customerId);
            CartItem cartItem;
            var product = productService.GetBracelet(productId);
            if (product.ProductType == ProductTypeEnum.CustomTextBracelet || 
                product.ProductType == ProductTypeEnum.CustomLetterBracelet)
            {
                cartItem = new CustomTextBraceletCartItem
                {
                    ProductId = productId,
                    BraceletSize = braceletSize,
                    CustomText = customText
                };
            }
            else
            {
                cartItem = new BraceletCartItem
                {
                    ProductId = productId,
                    BraceletSize = braceletSize,
                    BraceletSize2 = braceletSize2
                };
            }
            cartItem.Cart = cart;
            cartItem.CartId = cart.Id;
            cartItem.Quantity = quantity;
            cartItem.AddedDate = Helper.Now;

            cart.CartItems.Add(cartItem);
            context.CartItems.Add(cartItem);
            context.SaveChanges();
        }

        public void AddToCart(int customerId, int cartItemId, int beadTypeId, int secondaryBeadTypeId, CustomBraceletStyleEnum styleType, int[] componentIds, BraceletSizeEnum braceletSize, int[] complementaryProducts)
        {
            var cart = GetCustomerCart(customerId);
            var cartItem = context.CustomBraceletCartItems
                .Include(c => c.Components)
                .Include(c => c.CartItemComplementaryProducts)
                .Where(c => c.Id == cartItemId &&
                            c.Cart.CustomerId == customerId).FirstOrDefault();
            if (cartItem == null)
            {
                cartItem = new CustomBraceletCartItem
                {
                    Cart = cart,
                    AddedDate = Helper.Now,
                    Quantity = 1
                };
            }
            else
            {
                context.RemoveRange(cartItem.Components);
                cartItem.Components.Clear();
                context.RemoveRange(cartItem.CartItemComplementaryProducts);
                cartItem.CartItemComplementaryProducts.Clear();
            }

            cartItem.BraceletSize = braceletSize;
            if (beadTypeId == 0 || secondaryBeadTypeId == 0)
            {
                cartItem.StyleType = CustomBraceletStyleEnum.Simple;
                cartItem.BeadTypeId = beadTypeId == 0 ? secondaryBeadTypeId : beadTypeId;
            }
            else
            {
                cartItem.StyleType = styleType;
                cartItem.BeadTypeId = beadTypeId;
                cartItem.SecondaryBeadTypeId = secondaryBeadTypeId;
            }

            var componentIndex = 1;
            foreach (var componentId in componentIds)
            {
                var component = new CustomBraceletComponent
                {
                    CartItem = cartItem,
                    ComponentId = componentId,
                    Position = componentIndex++
                };
                context.CustomBraceletComponents.Add(component);
                cartItem.Components.Add(component);
            }
            cart.CartItems.Add(cartItem);
            cartItem.CartItemComplementaryProducts = complementaryProducts.Select(id =>
                new CartItemComplementaryProduct
                {
                    ComplementaryProductId = id,
                    CartItem = cartItem
                }).ToList();
            context.CartItemComplementaryProducts.AddRange(cartItem.CartItemComplementaryProducts);
            if (cartItemId <= 0)
            {
                context.CustomBraceletCartItems.Add(cartItem);
            }
            context.SaveChanges();
        }

        public void AddToCart(int customerId, BraceletTypeEnum braceletType, string knotColor, string string1Color, string string2Color, string string3Color, string flap1Color, string flap2Color)
        {
            var cart = GetCustomerCart(customerId);
            var cartItem = new StringBraceletCartItem
            {
                BraceletType = braceletType,
                KnotColor = knotColor, 
                StringColor1 = string1Color,
                StringColor2 = string2Color, 
                StringColor3 = string3Color,
                FlapColor1 = flap1Color,
                FlapColor2 = flap2Color
            };
            cartItem.Cart = cart;
            cartItem.CartId = cart.Id;
            cartItem.Quantity = 1;
            cartItem.AddedDate = Helper.Now;

            cart.CartItems.Add(cartItem);
            context.CartItems.Add(cartItem);
            context.SaveChanges();
        }

        public void ChangeItemQuantity(int customerId, int cartItemId, int quantity)
        {
            var cart = GetCustomerCart(customerId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null) return;
            cartItem.Quantity = quantity;
            context.SaveChanges();
        }

        public void ChangeItemSize(int customerId, int cartItemId, BraceletSizeEnum braceletSize, bool isSize2)
        {
            var cart = GetCustomerCart(customerId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null) return;
            if (isSize2)
            {
                if (cartItem is BraceletCartItem)
                {
                    (cartItem as BraceletCartItem).BraceletSize2 = braceletSize;
                }
            }
            else if (cartItem is IBraceletWithSize)
            {
                (cartItem as IBraceletWithSize).BraceletSize = braceletSize;
            }
            context.SaveChanges();
        }

        public void RemoveFromCart(int customerId, int cartItemId)
        {
            var cart = GetCustomerCart(customerId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null) return;
            cart.CartItems.Remove(cartItem);
            context.CartItems.Remove(cartItem);
            context.SaveChanges();
        }

        public Result ApplyCoupon(int customerId, string couponCode)
        {
            var coupon = context.Coupons.FirstOrDefault(c => c.Code.ToUpper() == couponCode.ToUpper());
            if (coupon == null) return new Result("Helytelen kuponkód!");
            if (coupon.EndDate.HasValue && coupon.EndDate.Value < Helper.Now) return new Result("Ez a kupon már lejárt!");
            if (coupon.MaxUsageCount.HasValue && coupon.UsageCount >= coupon.MaxUsageCount.Value) return new Result("Ez a kupon már nem használható!");
            var cart = GetCustomerCart(customerId);
            cart.Coupon = coupon;
            context.SaveChanges();
            return new Result();
        }

        public void RemoveCouponFromCart(int customerId)
        {
            var cart = GetCustomerCart(customerId);
            cart.Coupon = null;
            cart.CouponId = null;
            context.SaveChanges();
        }

        private decimal GetCartTotalWithoutShipping(Cart cart)
        {
            decimal total = 0;
            int couponPercentage = cart.Coupon?.Percentage ?? 0;
            decimal lowestPrice = int.MaxValue;
            foreach (var item in cart.CartItems)
            {
                var itemPrice = item.ItemPrice;
                var discounted = item.ItemOriginalPrice.HasValue;
                if (!discounted && couponPercentage > 0)
                {
                    itemPrice -= Math.Round(itemPrice * couponPercentage / 100);
                }
                itemPrice += item.CartItemComplementaryProducts.Sum(ci => ci.ComplementaryProduct.Price);
                if (itemPrice < lowestPrice)
                {
                    lowestPrice = itemPrice;
                }
                total += itemPrice * item.Quantity;
            }
            if (cart.Coupon != null)
            {
                if (cart.Coupon.Value != null && (cart.Coupon.MinCartValue == null || total >= cart.Coupon.MinCartValue.Value))
                {
                    total -= cart.Coupon.Value.Value;
                }
                else if (cart.Coupon.GetOneFreeMinimumQuantity > 0 && cart.CartItems.Sum(ci => ci.Quantity) >= cart.Coupon.GetOneFreeMinimumQuantity)
                {
                    total -= lowestPrice;
                }
            }
            return total;
        }
    }
}
