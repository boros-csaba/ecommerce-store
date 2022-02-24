using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class CustomBraceletOrderItem: OrderItem, IBraceletWithSize
    {
        public int BeadTypeId { get; set; }
        public Component BeadType { get; set; }
        public int? SecondaryBeadTypeId { get; set; }
        public Component SecondaryBeadType { get; set; }
        public CustomBraceletStyleEnum StyleType { get; set; }
        public List<CustomBraceletComponent> Components { get; set; }
        public BraceletSizeEnum? BraceletSize { get; set; }
        public override string ProductIdString => "custom-bracelet";
        public override string Name
        {
            get
            {
                if (StyleType == CustomBraceletStyleEnum.Simple)
                    return $"Egyedi {BeadType.Name.ToLower()} karkötő";
                else
                    return $"Egyedi {BeadType.Name.ToLower()} és {SecondaryBeadType.Name.ToLower()} karkötő";
            }
        }
    }
}
