using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class VisitorViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int Order { get; set; }
        public DateTime FirstAction { get; set; }
        public DateTime LastAction { get; set; }
        public int ActionsCount { get; set; }
        public int WishlistCount { get; set; }
        public int CartItemsCount { get; set; }
        public int OrdersCount { get; set; }
    }
}
