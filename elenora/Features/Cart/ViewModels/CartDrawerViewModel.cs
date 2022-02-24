using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CartDrawerViewModel
    {
        public List<Promotion> Promotions { get; set; }
        public CartViewModel Cart { get; set; }
    }
}
