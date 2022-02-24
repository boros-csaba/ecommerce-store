using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CategoryListViewModel
    {
        public string CategoryName { get; set; }
        public List<ProductListItemViewModel> Products { get; set; }
        public string Description { get; set; }
    }
}
