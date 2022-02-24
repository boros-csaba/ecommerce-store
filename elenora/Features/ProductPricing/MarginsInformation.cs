using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    public class MarginsInformation
    {
        public decimal MinTotal { get; set; }
        public decimal MaxTotal { get; set; }
        public string Description { get; set; }
        public bool MissingInformation { get; set; }
    }
}
