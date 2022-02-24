using elenora.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class SearchSuggestion
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public SearchSuggestionTypeEnum Type { get; set; } 
        public string ImageUrl { get; set; }
        public string SearchLinkUrl { get; set; }
    }
}
