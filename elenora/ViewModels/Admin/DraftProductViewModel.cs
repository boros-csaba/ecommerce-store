using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class DraftProductViewModel
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string SecondaryName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Components { get; set; }
        public decimal MarginsMinTotal { get; set; }
        public decimal MarginsMaxTotal { get; set; }
        public string MarginsDescription { get; set; }
        public bool MarginsMissingInformation { get; set; }
    }
}
