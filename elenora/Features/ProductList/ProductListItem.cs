using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList
{
    public class ProductListItem
    {
        public int ProductId { get; set; }
        public string ProductIdString { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public bool SoldOut { get; set; }
        public bool InWishlist { get; set; }
        public int SortingWeight { get; set; }
        public ProductCategoryTypeEnum CategoryType { get; set; }
        public int DiscountPercentage
        {
            get
            {
                if (OriginalPrice == null) return 0;
                return (int)Math.Floor((OriginalPrice.Value - Price) / OriginalPrice.Value * 100);
            }
        }
    }
}
