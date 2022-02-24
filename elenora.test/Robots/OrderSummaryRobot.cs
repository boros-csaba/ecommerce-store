using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace elenora.test.Robots
{
    public class OrderSummaryRobot:RobotBase
    {
        public OrderSummaryRobot(IWebDriver driver): base(driver) { }

        public void SubmitOrder()
        {
            ClickItemWithText("RENDELÉS LEADÁSA");
        }
    }
}
