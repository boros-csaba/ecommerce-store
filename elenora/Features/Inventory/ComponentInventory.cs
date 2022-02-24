using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.Inventory
{
    public class ComponentInventory
    {
        public int ComponentId { get; set; }
        public string Name { get; set; }
        public string ComponentImage { get; set; }
        public int Quantity { get; set; }
        public int SoldLast30Days { get; set; }
        public int ContainingBracelets { get; set; }
        public string Sources { get; set; }
        public int ImagesQuality { get; set; }
        public string Remark { get; set; }
    }
}
