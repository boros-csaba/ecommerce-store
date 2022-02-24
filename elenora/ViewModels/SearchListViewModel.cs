using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class SearchListViewModel
    {
        public string SearchTerm { get; set; }
        public List<ProductListItemViewModel> Products { get; set; }
    }
}
