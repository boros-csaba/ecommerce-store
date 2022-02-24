using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class BraceletSizeSelectorModel
    {
        public ProductTypeEnum ProductType { get; set; }
        public bool Inline { get; set; }
        public BraceletSizeEnum? Size { get; set; }
        public BraceletSizeEnum? Size2 { get; set; }
        public int? CartItemId { get; set; }
        public bool ShowHelpIcon { get; set; }
        public string UniqueId { get; set; }
    }
}
