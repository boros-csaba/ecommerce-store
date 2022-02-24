using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class CustomTextBraceletCartItem : CartItem, IBraceletWithSize
    {
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public string CustomText { get; set; }
        public override string Name => @$"{Product.Name} ""{CustomText}"" felirattal";
        public override decimal ItemPrice => Product.Price.Price;
        public override decimal? ItemOriginalPrice => Product.Price.OriginalPrice;
        public BraceletSizeEnum? BraceletSize { get; set; }
    }
}
