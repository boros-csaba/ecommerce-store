using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class MineralsListItemViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ArticlesDescription { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
