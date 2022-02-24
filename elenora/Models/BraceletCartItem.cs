using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class BraceletCartItem: CartItem, IBraceletWithSize
    {
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public BraceletSizeEnum? BraceletSize { get; set; }
        public BraceletSizeEnum? BraceletSize2 { get; set; }
        public override string Name => Product.Name;
        public override decimal ItemPrice => Product.Price.Price;
        public override decimal? ItemOriginalPrice => Product.Price.OriginalPrice;
    }
}
