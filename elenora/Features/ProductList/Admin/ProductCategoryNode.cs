using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList.Admin
{
    public class ProductCategoryNode
    {
        public string Name { get; set; }
        public string IdString { get; set; }
        public bool IsCategory { get; set; } = true;
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Components { get; set; }
        public decimal MarginsMinTotal { get; set; }
        public decimal MarginsMaxTotal { get; set; }
        public string MarginsDescription { get; set; }
        public bool MarginsMissingInformation { get; set; }
        public int TotalCount { get; set; }
        public int ActiveProductsCount { get; set; }
        public int SoldOutProductsCount { get; set; }
        public int PendingProductsCount { get; set; }
        public List<ProductCategoryNode> Children { get; set; } = new List<ProductCategoryNode>();
        public bool ButtonsVisible { get; set; }
        public string Status { get; set; }
        public int Feed { get; set; }
    }
}
