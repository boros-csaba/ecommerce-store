using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace elenora.test.Robots
{
    public class CheckoutCartRobot : RobotBase
    {
        public CheckoutCartRobot(IWebDriver driver) : base(driver) { }

        public ShippingModeRobot AddShippingMode()
        {
            ClickItemWithText("TOVÁBB A SZÁLLÍTÁSI MÓDOKHOZ");
            Thread.Sleep(1000);
            return new ShippingModeRobot(driver);
        }

        public ShippingModeRobot JumpToShippingModeCheckoutStep()
        {
            ClickItemWithText("2");
            return new ShippingModeRobot(driver);
        }

        public CheckoutCartRobot AddCoupon(string code)
        {
            InputTextByClass(code, "coupon-input");
            Thread.Sleep(3000);
            return this; 
        }

        public CheckoutCartRobot TotalShouldBe(int value)
        {
            var total = GetIntValueByCssSelector(".cart-total");
            Assert.Equal(value, total);
            return this;
        }
    }
}
