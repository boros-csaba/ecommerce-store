using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.Inventory
{
    public class ProductsInventoryReportItem
    {
        public string IdString { get; set; }
        public string Category { get; set; }
        public bool SoldOut { get; set; }
        public bool Active { get; set; }
    }
}
