using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string PrimaryName { get; set; }
        public string SecondaryName { get; set; }
        public string MainImageUrl { get; set; }
        public ProductStateEnum State { get; set; }
        public int UniqueViews { get; set; }
    }
}
