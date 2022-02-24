using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using elenora.test.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace elenora.test
{
    public class CartTests
    {
        private readonly DataContext context;
        private readonly ICustomerService customerService;
        private readonly ICartService cartService;
        private readonly Mock<IEmailService> emailServiceMock;
        private readonly CartRepository cartRepository;

        public CartTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new DataContext(options);

            cartRepository = new CartRepository();
            cartService = new CartService(null, context, cartRepository, null);
            emailServiceMock = new Mock<IEmailService>();
            customerService = new CustomerService(context, emailServiceMock.Object);
        }

        [Fact]
        public void EmptyCart()
        {
            cartRepository.Add(new Cart
            {
                CustomerId = 1,
                CartItems = new List<CartItem>()
            });

            var total = cartService.GetCartTotal(1);
            var coupon = cartService.GetCouponValue(1);
            var shipping = cartService.GetShippingPrice(1);

            Assert.Equal(990, total);
            Assert.Equal(0, coupon);
            Assert.Equal(990, shipping);
        }

        [Theory]
        [InlineData(5490, 1, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, 6480)]
        [InlineData(5490, 1, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, 6480)]
        [InlineData(5490, 1, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, 6880)]
        [InlineData(5490, 1, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, 6880)]
        [InlineData(5490, 1, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, 7570)]
        [InlineData(5490, 2, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, 10980)]
        [InlineData(5490, 2, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, 10980)]
        [InlineData(5490, 2, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, 10980)]
        [InlineData(5490, 2, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, 10980)]
        [InlineData(5490, 2, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, 10980)]
        [InlineData(3000, 3, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, 9000)]
        [InlineData(3000, 3, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, 9000)]
        [InlineData(3000, 3, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, 9000)]
        [InlineData(3000, 3, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, 9000)]
        [InlineData(3000, 3, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, 9000)]
        public void CartWithSingleProduct(decimal price, int quantity, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod, decimal expectedShipping, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && paymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShipping == 0)
            {
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
                expectedShipping = Settings.GLS_PAYMENT_PRICE;
            }

            cartRepository.Add(new Cart
            {
                CustomerId = 1,
                ShippingMethod = shippingMethod,
                PaymentMethod = paymentMethod,
                CartItems = new List<CartItem>
                {
                    new BraceletCartItem
                    {
                        Product = new Bracelet { 
                            //Price = price 
                        },
                        Quantity = quantity,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    }
                }
            });

            var total = cartService.GetCartTotal(1);
            var coupon = cartService.GetCouponValue(1);
            var shipping = cartService.GetShippingPrice(1);

            Assert.Equal(expectedTotal, total);
            Assert.Equal(0, coupon);
            Assert.Equal(expectedShipping, shipping);
        }

        [Theory]
        [InlineData(5490, 1000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, 7480)]
        [InlineData(5490, 1000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, 7480)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, 7880)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, 7880)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, 8570)]
        [InlineData(5490, 5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, 10980)]
        [InlineData(5490, 5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, 10980)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, 10980)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, 10980)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, 10980)]
        [InlineData(3000, 6000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, 9000)]
        [InlineData(3000, 6000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, 9000)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, 9000)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, 9000)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, 9000)]
        public void CartWithMultipleProduct(decimal price1, int price2, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod, decimal expectedShipping, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && paymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShipping == 0)
            {
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
                expectedShipping = Settings.GLS_PAYMENT_PRICE;
            }

            cartRepository.Add(new Cart
            {
                CustomerId = 1,
                ShippingMethod = shippingMethod,
                PaymentMethod = paymentMethod,
                CartItems = new List<CartItem>
                {
                    new BraceletCartItem
                    {
                        Product = new Bracelet { 
                            //Price = price1 
                        },
                        Quantity = 1,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    },
                    /*new HoroscopeBraceletCartItem
                    {
                        HoroscopeBracelet = new HoroscopeBracelet { 
                            //Price = price2 
                        },
                        Quantity = 1,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    }*/
                }
            });

            var total = cartService.GetCartTotal(1);
            var coupon = cartService.GetCouponValue(1);
            var shipping = cartService.GetShippingPrice(1);

            Assert.Equal(expectedTotal, total);
            Assert.Equal(0, coupon);
            Assert.Equal(expectedShipping, shipping);
        }

        [Theory]
        [InlineData(5490, 1000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, -549, 6931)]
        [InlineData(5490, 1000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, -549, 6931)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, -549, 7331)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, -549, 7331)]
        [InlineData(5490, 1000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, -549, 8021)]
        [InlineData(5490, 5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, -549, 10431)]
        [InlineData(5490, 5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, -549, 10431)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, -549, 10431)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, -549, 10431)]
        [InlineData(5490, 5490, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, -549, 10431)]
        [InlineData(3000, 6000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, -300, 9690)]
        [InlineData(3000, 6000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, -300, 9690)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, -300, 10090)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, -300, 10090)]
        [InlineData(3000, 6000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, -300, 10780)]
        public void CartWithPercentageCoupon(decimal normalPrice, int discoutPrice, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod, decimal expectedShipping, decimal expectedCoupon, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && paymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShipping == 0)
            {
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
                expectedShipping = Settings.GLS_PAYMENT_PRICE;
            }

            cartRepository.Add(new Cart
            {
                CustomerId = 1,
                ShippingMethod = shippingMethod,
                PaymentMethod = paymentMethod,
                Coupon = new Coupon { Percentage = 10 },
                CartItems = new List<CartItem>
                {
                    new BraceletCartItem
                    {
                        Product = new Bracelet { 
                            //Price = discoutPrice, 
                            //OriginalPrice = discoutPrice + 1 
                        },
                        Quantity = 1,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    },
                    /*new HoroscopeBraceletCartItem
                    {
                        HoroscopeBracelet = new HoroscopeBracelet { 
                            //Price = normalPrice,  
                        },
                        Quantity = 1,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    }*/
                }
            });

            var total = cartService.GetCartTotal(1);
            var coupon = cartService.GetCouponValue(1);
            var shipping = cartService.GetShippingPrice(1);

            Assert.Equal(expectedTotal, total);
            Assert.Equal(expectedCoupon, coupon);
            Assert.Equal(expectedShipping, shipping);
        }

        [Theory]
        [InlineData(5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, 0, 6480)]
        [InlineData(5490, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, 0, 6480)]
        [InlineData(5490, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, 0, 6880)]
        [InlineData(5490, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, 0, 6880)]
        [InlineData(5490, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, 0, 7570)]
        [InlineData(15000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 0, -2000, 13000)]
        [InlineData(15000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 0, -2000, 13000)]
        [InlineData(15000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 0, -2000, 13000)]
        [InlineData(15000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 0, -2000, 13000)]
        [InlineData(15000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 0, -2000, 13000)]
        [InlineData(10000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, 990, -2000, 8990)]
        [InlineData(10000, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, 990, -2000, 8990)]
        [InlineData(10000, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, 1390, -2000, 9390)]
        [InlineData(10000, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, 1390, -2000, 9390)]
        [InlineData(10000, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, 2080, -2000, 10080)]
        public void CartWithFixedValueCoupon(decimal price, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod, decimal expectedShipping, decimal expectedCoupon, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && paymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShipping == 0)
            {
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
                expectedShipping = Settings.GLS_PAYMENT_PRICE;
            }

            cartRepository.Add(new Cart
            {
                CustomerId = 1,
                ShippingMethod = shippingMethod,
                PaymentMethod = paymentMethod,
                Coupon = new Coupon { 
                    MinCartValue = 10000,
                    Value = 2000
                },
                CartItems = new List<CartItem>
                {
                    new BraceletCartItem
                    {
                        Product = new Bracelet { 
                            //Price = price 
                        },
                        Quantity = 1,
                        CartItemComplementaryProducts = new List<CartItemComplementaryProduct>()
                    }
                }
            });

            var total = cartService.GetCartTotal(1);
            var coupon = cartService.GetCouponValue(1);
            var shipping = cartService.GetShippingPrice(1);

            Assert.Equal(expectedTotal, total);
            Assert.Equal(expectedCoupon, coupon);
            Assert.Equal(expectedShipping, shipping);
        }
    }
}
