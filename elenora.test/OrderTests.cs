using elenora.Features.Cart;
using elenora.Models;
using elenora.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace elenora.test
{
    public class OrderTests
    {
        private readonly DataContext context;
        private readonly ICustomerService customerService;
        private readonly ICartService cartService;
        private readonly IOrderService orderService;

        public OrderTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new DataContext(options);

            var emailServiceMock = new Mock<IEmailService>();
            customerService = new CustomerService(context, emailServiceMock.Object);
            var cartRepository = new CartRepository(null, context);
            cartService = new CartService(null, context, cartRepository, null);
            orderService = new OrderService(null, null, null, context, new CartService(null, context, cartRepository, null), emailServiceMock.Object, null);
        }

        [Fact]
        public void NewOrder_ShouldNotBeNull()
        {
            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());

            var activeOrder = orderService.GetCustomerActiveOrder(customer.Id);

            Assert.NotNull(activeOrder);
        }

        [Fact]
        public void SameActiveOrder_ShouldBeReturnedForCustomer()
        {
            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());

            var activeOrder = orderService.GetCustomerActiveOrder(customer.Id);
            var activeOrderAgain = orderService.GetCustomerActiveOrder(customer.Id);

            Assert.Equal(activeOrder.Id, activeOrderAgain.Id);
        }

        [Theory]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, true, 0 ,12390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, false, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, false, 0, 12390)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 0, 12390)]
        public void ChangeOrder_PaymentMethod_OverThreshold_Test(ShippingMethodEnum originalShippingMethod, PaymentMethodEnum originalPaymentMethod, ShippingMethodEnum newShippingMethod, PaymentMethodEnum newPaymentMethod, bool valid, decimal expectedShippingPrice, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && newPaymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShippingPrice == 0)
            {
                expectedShippingPrice = Settings.GLS_PAYMENT_PRICE;
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
            }

            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());
            var order = orderService.GetCustomerActiveOrder(customer.Id);
            SetupOrder(customer, originalShippingMethod, originalPaymentMethod, 5900, 3);

            if (valid)
            {
                orderService.ChangePaymentMethod(order.OrderId, customer.CookieId, newPaymentMethod);

                order = orderService.GetOrderById(order.Id);
                Assert.Equal(newShippingMethod, order.ShippingMethod);
                Assert.Equal(newPaymentMethod, order.PaymentMethod);
                Assert.Equal(expectedShippingPrice, order.ShippingPrice);
                Assert.Equal(expectedTotal, order.Total);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => orderService.ChangePaymentMethod(order.OrderId, customer.CookieId, newPaymentMethod));
            }
        }

        [Theory]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, true, 990, 9390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, true, 990, 9390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 2080, 10480)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.Barion, true, 990, 9390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, true, 990, 9390)]
        [InlineData(ShippingMethodEnum.GlsCsomagpont, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 2080, 10480)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, true, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, true, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 2080, 10480)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, true, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, true, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 2080, 10480)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.Barion, false, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.BankTransfer, false, 1390, 9790)]
        [InlineData(ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, ShippingMethodEnum.GLS, PaymentMethodEnum.PayAtDelivery, true, 2080, 10480)]
        public void ChangeOrder_PaymentMethod_BellowThreshold_Test(ShippingMethodEnum originalShippingMethod, PaymentMethodEnum originalPaymentMethod, ShippingMethodEnum newShippingMethod, PaymentMethodEnum newPaymentMethod, bool valid, decimal expectedShippingPrice, decimal expectedTotal)
        {
            if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT && newPaymentMethod == PaymentMethodEnum.PayAtDelivery && expectedShippingPrice == 0)
            {
                expectedShippingPrice = Settings.GLS_PAYMENT_PRICE;
                expectedTotal += Settings.GLS_PAYMENT_PRICE;
            }

            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());
            var order = orderService.GetCustomerActiveOrder(customer.Id);
            SetupOrder(customer, originalShippingMethod, originalPaymentMethod, 12000, 1);

            if (valid)
            {
                orderService.ChangePaymentMethod(order.OrderId, customer.CookieId, newPaymentMethod);

                order = orderService.GetOrderById(order.Id);
                Assert.Equal(newShippingMethod, order.ShippingMethod);
                Assert.Equal(newPaymentMethod, order.PaymentMethod);
                Assert.Equal(expectedShippingPrice, order.ShippingPrice);
                Assert.Equal(expectedTotal, order.Total);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => orderService.ChangePaymentMethod(order.OrderId, customer.CookieId, newPaymentMethod));
            }
        }

        private void SetupOrder(Customer customer, ShippingMethodEnum shippingMethod, PaymentMethodEnum paymentMethod, decimal price, int quantity)
        {
            var productId = Helper.CreateProduct(context, price);
            cartService.AddToCart(customer.Id, productId, quantity, null, null, null);

            Helper.CreatePercentageCoupon(context, 30, "coupon1");
            cartService.ApplyCoupon(customer.Id, "coupon1");
            var cart = cartService.GetCustomerCart(customer.Id);

            var order = orderService.GetCustomerActiveOrder(customer.Id);
            order.Status = OrderStatusEnum.OrderPlaced;
            order.ShippingMethod = shippingMethod;
            order.PaymentMethod = paymentMethod;
            order.Coupon = cart.Coupon;
            order.ShippingPrice = cartService.GetShippingPrice(0.7m * price * quantity, shippingMethod, paymentMethod);
            order.CouponValue = -0.3m * price * quantity; 
            order.OrderItems = cart.CartItems.Select(c => new BraceletOrderItem
            {
                Order = order,
                UnitPrice = price,
                Quantity = quantity
            }).ToList<OrderItem>();
            order.Total = cartService.GetCartTotal(cart);

            context.SaveChanges();
        }
    }
}
