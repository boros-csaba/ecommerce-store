using elenora.test.Robots;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace elenora.test
{
    public class E2E
    {
        private readonly IWebDriver driver;

        public E2E()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
        }

        [Fact(Skip = "UI")]
        //[Fact]
        public void Scenario()
        {
            new Robot(driver)
                .CloseCookiePopup()
                .GoToProduct("Timoti")
                    .AddToCart()
                        .TotalShouldBe(4990)
                        .Checkout()
                            .TotalShouldBe(4990)
                            .GoToHomePage()
                .GoToProduct("Our World")
                    .IncreaseQuantity()
                    .AddToCart()
                        .TotalShouldBe(20970)
                        .Checkout()
                            .TotalShouldBe(20970)
                            .AddCoupon("ELSO100")
                            .TotalShouldBe(16776)
                            .AddShippingMode()
                                .EmailShouldBe("")
                                .InputEmailAddress("boros.csaba94@gmail.com")
                                .JumpToCartCheckoutStep()
                            .JumpToShippingModeCheckoutStep()
                                .EmailShouldBe("boros.csaba94@gmail.com")
                                .AddBillingInformation()
                                    .InputName("Boros Csaba")
                                    .InputZip("1193")
                                    .InputCity("Budapest")
                                    .InputAddress("Szigligeti utca 1, 9/26")
                                    .InputRemark("Valami megjegyzés")
                                    .SetDifferentShippingAddress(true)
                                    .InputShippingName("Teszt Elek")
                                    .InputShippingZip("1234")
                                    .InputShippingCity("Eger")
                                    .InputShippingAddress("Fő utca 1")
                                    .JumpToShippingModeCheckoutStep()
                                .JumpToBillingInformationCheckoutStep()
                                    .NameShouldBe("Boros Csaba")
                                    .ZipShouldBe("1193")
                                    .CityShouldBe("Budapest")
                                    .AddressShouldBe("Szigligeti utca 1, 9/26")
                                    .RemarkShouldBe("Valami megjegyzés")
                                    .DifferentShippingAddressShouldBe(true)
                                    .ShippingNameShouldBe("Teszt Elek")
                                    .ShippingZipShouldBe("1234")
                                    .ShippingCityShouldBe("Eger")
                                    .ShippingAddressShouldBe("Fő utca 1")
                                    .SummarizeOrder()
                                        .SubmitOrder();

            /* TODO
                Posta Pont
                Zip -> City
                Invalid email
            */
        }

    }
}
