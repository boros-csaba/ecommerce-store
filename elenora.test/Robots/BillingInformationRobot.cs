using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace elenora.test.Robots
{
    public class BillingInformationRobot: RobotBase
    {
        public BillingInformationRobot(IWebDriver driver) : base(driver) { }

        public BillingInformationRobot InputName(string name)
        {
            InputTextById(name, "name");
            return this;
        }

        public BillingInformationRobot InputZip(string zip)
        {
            InputTextById(zip, "zipCode");
            return this;
        }

        public BillingInformationRobot InputCity(string city)
        {
            InputTextById(city, "city");
            return this;
        }

        public BillingInformationRobot InputAddress(string address)
        {
            InputTextById(address, "address");
            return this;
        }

        public BillingInformationRobot InputRemark(string remark)
        {
            InputTextById(remark, "remark");
            return this;
        }

        public BillingInformationRobot SetDifferentShippingAddress(bool differentShippingAddress)
        {
            if (differentShippingAddress) CheckCheckbox("differentShippingAddress");
            else UncheckCheckbox("differentShippingAddress");
            return this;
        }

        public BillingInformationRobot InputShippingName(string name)
        {
            InputTextById(name, "shipping-name");
            return this;
        }

        public BillingInformationRobot InputShippingZip(string zip)
        {
            InputTextById(zip, "shipping-zipCode");
            return this;
        }

        public BillingInformationRobot InputShippingCity(string city)
        {
            InputTextById(city, "shipping-city");
            return this;
        }

        public BillingInformationRobot InputShippingAddress(string address)
        {
            InputTextById(address, "shipping-address");
            return this;
        }

        public ShippingModeRobot JumpToShippingModeCheckoutStep()
        {
            ClickItemWithText("2");
            return new ShippingModeRobot(driver);
        }

        public OrderSummaryRobot SummarizeOrder()
        {
            ClickItemWithText("RENDELÉS ÖSSZEGZÉSE");
            return new OrderSummaryRobot(driver);
        } 

        public BillingInformationRobot NameShouldBe(string name)
        {
            CheckInputValue(name, "name");
            return this;
        }

        public BillingInformationRobot ZipShouldBe(string zip)
        {
            CheckInputValue(zip, "zipCode");
            return this;
        }

        public BillingInformationRobot CityShouldBe(string city)
        {
            CheckInputValue(city, "city");
            return this;
        }

        public BillingInformationRobot AddressShouldBe(string address)
        {
            CheckInputValue(address, "address");
            return this;
        }

        public BillingInformationRobot RemarkShouldBe(string remark)
        {
            CheckInputValue(remark, "remark");
            return this;
        }

        public BillingInformationRobot DifferentShippingAddressShouldBe(bool isChecked)
        {
            CheckCheckbox(isChecked, "differentShippingAddress");
            return this;
        }

        public BillingInformationRobot ShippingNameShouldBe(string shippingName)
        {
            CheckInputValue(shippingName, "shipping-name");
            return this;
        }

        public BillingInformationRobot ShippingZipShouldBe(string zip)
        {
            CheckInputValue(zip, "shipping-zipCode");
            return this;
        }

        public BillingInformationRobot ShippingCityShouldBe(string city)
        {
            CheckInputValue(city, "shipping-city");
            return this;
        }

        public BillingInformationRobot ShippingAddressShouldBe(string address)
        {
            CheckInputValue(address, "shipping-address");
            return this;
        }
    }
}
