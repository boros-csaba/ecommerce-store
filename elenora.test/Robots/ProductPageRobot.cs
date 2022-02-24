using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace elenora.test.Robots
{
    public class ProductPageRobot
    {
        private readonly IWebDriver driver;

        public ProductPageRobot(IWebDriver driver)
        {
            this.driver = driver;
        }

        public CartDrawerRobot AddToCart()
        {
            Thread.Sleep(1000);
            driver.FindElement(By.Id("top-buy-button")).Click();
            Thread.Sleep(2000);
            return new CartDrawerRobot(driver);
        }

        public ProductPageRobot IncreaseQuantity()
        {
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName("number-input-plus")).Click();
            return this;
        }
    }
}
