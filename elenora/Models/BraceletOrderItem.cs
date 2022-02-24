using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class BraceletOrderItem: OrderItem, IBraceletWithSize
    {
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public BraceletSizeEnum? BraceletSize { get; set; }
        public BraceletSizeEnum? BraceletSize2 { get; set; }
        public override string ProductIdString => Product.IdString;
        public override string Name => Product.Name;
    }
}
