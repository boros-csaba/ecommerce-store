using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class BraceletDesignerBeadViewModel
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public bool SoldOut { get; set; }
        public bool IsPremium { get; set; }
        public List<ComplementaryProductViewModel> ComplementaryProducts { get; set; }
        public int[] ImageFrequencies { get; set; }
    }
}
