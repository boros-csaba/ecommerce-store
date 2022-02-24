using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public DateTime FirstSearch { get; set; }
        public DateTime LastSearch { get; set; }
        public int Count { get; set; }
    }
}
