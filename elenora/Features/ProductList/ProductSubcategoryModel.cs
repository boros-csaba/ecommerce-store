using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList
{
    public class ProductSubcategoryModel
    {
        public string Title { get; set; }
        public string IdString { get; set; }
        public string Description { get; set; }
        public List<ProductListItem> Products { get; set; } = new List<ProductListItem>();
    }
}
