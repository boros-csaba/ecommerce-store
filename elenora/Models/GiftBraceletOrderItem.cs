using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class GiftBraceletOrderItem : OrderItem
    {
        public PromotionEnum PromotionType { get; set; }
        public override string ProductIdString
        {
            get
            {
                if (PromotionType == PromotionEnum.GiftLavaBracelet) return "free-lava-bracelet";
                return "";
            }
        }
        public override string Name
        {
            get
            {
                if (PromotionType == PromotionEnum.GiftLavaBracelet) return "Ajándék lávakő karkötő";
                return "";
            }
        }
    }
}
