using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class HomePageViewModel
    {
        public List<ProductListItemViewModel> FeaturedProducts { get; set; }
        public List<ProductListItemViewModel> WomenBracelets { get; set; }
        public List<ProductListItemViewModel> MenBracelets { get; set; }
        public List<ProductListItemViewModel> PairBracelets { get; set; }
        public List<ProductListItemViewModel> DiscountedBracelets { get; set; }
    }
}
