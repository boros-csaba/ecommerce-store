using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class ActionViewModel
    {
        public int ActionsCount { get; set; }
        public DateTime FirstAction { get; set; }
        public DateTime LastAction { get; set; }
        public string Referrer { get; set; }
        public decimal CartValue { get; set; }
        public string CheckoutStatus { get; set; }
        public decimal OrderedValue { get; set; }
        public string Email { get; set; }
        public int CustomerId { get; set; }
    }
}
