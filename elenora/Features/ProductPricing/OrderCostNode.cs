using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    public class OrderCostNode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public bool AddToSum { get; set; }
        public string Error { get; set; }
        public List<OrderCostNode> Children { get; set; } = new List<OrderCostNode>();
    }
}
