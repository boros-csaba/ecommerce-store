using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace elenora.test.Robots
{
    public class Robot : RobotBase
    {
        private static string url = "https://192.168.0.105:45455/";

        public Robot(IWebDriver driver) : base(driver)
        {
            driver.Navigate().GoToUrl(url);
        }

        public Robot CloseCookiePopup()
        {
            ClickItemWithText("Értem");
            return this;
        }

        public ProductPageRobot GoToProduct(string name)
        {
            ClickItemWithText(name);
            return new ProductPageRobot(driver);
        }

    }
}
