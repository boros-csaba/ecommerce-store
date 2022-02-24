using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CheckoutModel
    {
        public int CustomerId { get; set; }
        public int OrderStatus { get; set; }
        public string Email { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public string BillingName { get; set; }
        public string BillingZip { get; set; }
        public string BillingCity { get; set; }
        public string BillingAddress { get; set; }
        public string Phone { get; set; }
        public bool DifferentShippingAddress { get; set; }
        public string ShippingName { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Total { get; set; }
        public string Remark { get; set; }
        public CartViewModel Cart { get; set; }
        public int OrderId { get; set; }
        public string ShippingPointAddressInformation { get; set; }

        public List<Promotion> Promotions { get; set; }
        public decimal OrderBusinessValue
        {
            get
            {
                var total = Cart.Total;
                var braceletsCount = Cart.CartItems.Sum(ci => ci.Quantity);

                if (PaymentMethod == PaymentMethodEnum.PayAtDelivery)
                    total -= 2080;
                else if (ShippingMethod == ShippingMethodEnum.GLS)
                    total -= 1390;

                total -= Settings.BRACELET_ENVELOPE_PRICE;
                total -= braceletsCount * Settings.BRACELET_BAG_COST;
                total -= braceletsCount * Settings.AVERAGE_BRACELET_COST;

                if (PaymentMethod == PaymentMethodEnum.BankTransfer) total = total / 2;
                else total = total * (decimal)0.8;

                return Math.Round(total * (decimal)0.73);
            }
        }

        public string GlsShippingPrice
        {
            get
            {
                var totalWithoutShipping = Total - Cart.ShippingPrice;
                if (totalWithoutShipping < Settings.FREE_SHIPPING_THRESHOLD || 
                    !Settings.FREE_SHIPPING_INCLUDES_PAYMENT)
                {
                    return "+" + Helper.GetFormattedMoney(Settings.GLS_PAYMENT_PRICE) + " Ft";
                }
                return string.Empty;
            }
        }
    }
}
