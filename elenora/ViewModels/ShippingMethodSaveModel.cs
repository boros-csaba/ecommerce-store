using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ShippingMethodSaveModel
    {
        public bool Validate { get; set; }
        public int ShippingMethod { get; set; }
        public string EmailAddress { get; set; }
        public string ShippingPointAddressInformation { get; set; }
    }
}
