using elenora.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface ISearchService
    {
        public List<SearchSuggestion> GetSearchSuggestions(string searchTerm);
        public ConcurrentDictionary<string, List<SearchSuggestion>> GetCacheInformation();
    }
}
