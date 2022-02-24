using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class BraceletDesignerViewModel
    {
        public int? CartItemId { get; set; }
        public int SelectedBeadId { get; set; }
        public int SelectedSecondaryBeadId { get; set; }
        public CustomBraceletStyleEnum StyleType { get; set; }
        public List<int> SelectedComponentIds { get; set; } = new List<int>();
        public List<int> SelectedComplementaryProductIds { get; set; } = new List<int>();
        public BraceletSizeEnum BraceletSize { get; set; }
        public List<Tuple<string, string>> ExampleImages { get; set; }
        public List<BraceletDesignerBeadViewModel> Beads { get; set; }
        public List<Tuple<int, string>> WhiteLetters { get; set; }
        public List<Tuple<int, string>> BlackLetters { get; set; }
        public string DeliveryTimeFaqAnswer { get; set; }
    }
}
