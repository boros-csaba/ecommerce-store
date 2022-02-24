using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace elenora.test.Robots
{
    public class CartDrawerRobot : RobotBase
    {
        public CartDrawerRobot(IWebDriver driver) : base(driver) { }

        public CheckoutCartRobot Checkout()
        {
            ClickItemWithText("PÉNZTÁR");
            return new CheckoutCartRobot(driver);
        }

        public CartDrawerRobot TotalShouldBe(int value)
        {
            var total = GetIntValueByCssSelector("#cart-drawer > div.cart-drawer-footer > div > h1.cart-total");
            Assert.Equal(value, total);
            return this;
        }
    }
}
