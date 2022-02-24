using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace elenora.test.Robots
{
    public class RobotBase
    {
        protected IWebDriver driver;

        public RobotBase(IWebDriver driver)
        {
            this.driver = driver;
        }

        public Robot GoToHomePage()
        {
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName("logo")).Click();
            return new Robot(driver);
        }

        protected void ClickItemWithText(string text)
        {
            Thread.Sleep(1000);
            driver.FindElement(By.XPath($"//*[text()='{text}']")).Click();
        }

        protected int GetIntValueByCssSelector(string cssSelector)
        {
            var totalText = driver.FindElement(By.CssSelector(cssSelector)).Text;
            totalText = totalText.Replace(" Ft", "").Replace(" ", "");
            return int.Parse(totalText);
        }

        protected int GetIntByClass(string cssClass)
        {
            var totalText = driver.FindElement(By.ClassName(cssClass)).Text;
            totalText = totalText.Replace(" Ft", "").Replace(" ", "");
            return int.Parse(totalText);
        }

        protected void InputTextById(string value, string id)
        {
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id)).SendKeys(value);
        }

        protected void InputTextByClass(string value, string cssClass)
        {
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName(cssClass)).SendKeys(value);
        }

        protected void CheckCheckbox(string id)
        {
            Thread.Sleep(1000);
            Assert.False(driver.FindElement(By.Id(id)).Selected);
            driver.FindElement(By.Id(id)).Click();
        }

        protected void UncheckCheckbox(string id)
        {
            Thread.Sleep(1000);
            Assert.True(driver.FindElement(By.Id(id)).Selected);
            driver.FindElement(By.Id(id)).Click();
        }

        protected void CheckInputValue(string expected, string id)
        {
            Thread.Sleep(1000);
            var value = driver.FindElement(By.Id(id)).GetAttribute("value");
            Assert.Equal(expected, value);
        }

        protected void CheckCheckbox(bool isChecked, string id)
        {
            Thread.Sleep(1000);
            Assert.Equal(isChecked, driver.FindElement(By.Id(id)).Selected);
        }
    }
}
