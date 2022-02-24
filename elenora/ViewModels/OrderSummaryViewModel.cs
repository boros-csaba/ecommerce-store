using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class OrderSummaryViewModel
    {
        public CartViewModel Cart { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public string Email { get; set; }
    }
}
