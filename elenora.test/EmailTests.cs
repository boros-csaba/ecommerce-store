using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using elenora.test.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace elenora.test
{
    public class EmailTests
    {
        private readonly DataContext context;
        private readonly ICustomerService customerService;
        private readonly IOrderService orderService;
        private readonly ICartService cartService;
        private readonly Mock<IEmailService> emailServiceMock;
        private readonly IConfiguration configuration;
        private readonly CartRepository cartRepository;

        public EmailTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new DataContext(options);

            emailServiceMock = new Mock<IEmailService>();
            customerService = new CustomerService(context, emailServiceMock.Object);
            cartRepository = new CartRepository();
            cartService = new CartService(null, context, cartRepository, null);
            orderService = new OrderService(null, null, null, context, cartService, emailServiceMock.Object, null);
            configuration = new Mock<IConfiguration>().Object;
        }

        [Fact]
        public void OrderPlacedEmail_ShouldContains_CorrectInforamtion()
        {
            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());
            var product = Helper.CreateProduct(context);
            cartService.AddToCart(customer.Id, product, 1, BraceletSizeEnum.M, null, null);
            var activeOrder = orderService.GetCustomerActiveOrder(customer.Id);
            orderService.CompleteShippingMethodStep(customer.Id, true, "email@gmail.com", ShippingMethodEnum.GLS, null);
            orderService.CompleteBillingInformationStep(customer.Id, true, "Boros Brigitta", "1234", "Város", "Pontos cím", "012345678", "Megjegyzés", PaymentMethodEnum.BankTransfer, false);
            
            orderService.PlaceOrder(activeOrder.Id, false);

            emailServiceMock.Verify(s => s.SendEmail("email@gmail.com", "Boros Brigitta", "Értesítés a leadott rendeléseddel kapcsolatban", It.IsAny<string>(), It.Is<string>(b => b.ToString().Contains("Kedves Brigitta,")), It.IsAny<string>()));
        }

        [Fact]
        public void AbandonedCartSequenceFirstEmail_ShouldContains_CorrectInforamtion()
        {
            var customer = customerService.GetOrCreateCustomer(Guid.NewGuid().ToString());
            var product = Helper.CreateProduct(context);
            cartService.AddToCart(customer.Id, product, 1, BraceletSizeEnum.M, null, null);
            var activeOrder = orderService.GetCustomerActiveOrder(customer.Id);
            orderService.CompleteShippingMethodStep(customer.Id, true, "boros.csaba94@gmail.com", ShippingMethodEnum.GLS, null);
            activeOrder.ModifiedDate = DateTime.Now.AddHours(-4);
            context.SaveChanges();

            orderService.SendAbandonedCartEmailSequences();

            emailServiceMock.Verify(s => s.SendEmail(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(b => b.ToString().Contains("Szia!")), It.IsAny<string>()));
        }

        [Theory]
        [InlineData("Boros Brigitta", "Kedves Brigitta,")]
        [InlineData("Nikoletta Bacsó-Sajtos", "Kedves Nikoletta,")]
        [InlineData("Bacsó-Sajtos Nikoletta", "Kedves Nikoletta,")]
        [InlineData("Hontiné Borza Klára", "Kedves Klára,")]
        [InlineData("Horváth Tibor Flóriánné", "Kedves Flóriánné,")]
        [InlineData("Palotai Patrícia", "Kedves Patrícia,")]
        [InlineData("Patrícia Palotai", "Kedves Patrícia,")]
        public void SalutationText_ShouldInclude_CorrectName(string name, string expectedSalutation)
        {
            var emailService = new EmailService(configuration, context);

            var salutation = emailService.GetSalutation(name);

            Assert.Equal(expectedSalutation, salutation);
        }
    }
    
}
