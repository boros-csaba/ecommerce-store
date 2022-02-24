using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class PopupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastDisplayed { get; set; }
        public int ShowCount { get; set; }
        public int ActionCount { get; set; }
        public int OrdersCount { get; set; }
        public int OrdersWithCouponCount { get; set; }
    }
}
