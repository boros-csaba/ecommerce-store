using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class CartViewModel
    {
        public string EmailAddress { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal CartValue { get; set; }
        public string CouponCode { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }
}
