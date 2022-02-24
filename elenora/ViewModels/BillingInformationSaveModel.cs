using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class BillingInformationSaveModel
    {
        public bool Validate { get; set; }
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
        public string Remark { get; set; }
        public int PaymentMethod { get; set; }
    }
}
