using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList
{
    public class ProductList
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ProductListItem> Items { get; set; }
        public int Count { get; set; }
    }
}
