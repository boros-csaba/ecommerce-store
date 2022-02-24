using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    [NotMapped]
    public class ProductPrice
    {
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? DiscountEndSeconds { get; set; }
    }
}
