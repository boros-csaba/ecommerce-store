using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ShippingDetailsViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public string ShippingName { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingAddressLine { get; set; }
        public CartViewModel Cart { get; set; }
    }
}
