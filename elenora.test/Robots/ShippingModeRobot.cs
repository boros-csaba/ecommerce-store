using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace elenora.test.Robots
{
    public class ShippingModeRobot: RobotBase
    {
        public ShippingModeRobot(IWebDriver driver): base(driver) { }

        public ShippingModeRobot InputEmailAddress(string emailAddress)
        {
            InputTextById(emailAddress, "email");
            return this;
        }

        public CheckoutCartRobot JumpToCartCheckoutStep()
        {
            ClickItemWithText("1");
            return new CheckoutCartRobot(driver);
        }

        public BillingInformationRobot JumpToBillingInformationCheckoutStep()
        {
            ClickItemWithText("3");
            return new BillingInformationRobot(driver);
        }

        public BillingInformationRobot AddBillingInformation()
        {
            ClickItemWithText("TOVÁBB A SZÁMLÁZÁSI ADATOKHOZ");
            return new BillingInformationRobot(driver);
        }

        public ShippingModeRobot EmailShouldBe(string emailAddress)
        {
            CheckInputValue(emailAddress, "email");
            return this;
        }
    }
}
