using elenora.Models;
using elenora.ViewModels;
using elenora.ViewModels.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace elenora.Services
{
    public class SearchService : ISearchService
    {
        private static ConcurrentDictionary<string, List<SearchSuggestion>> cache = new ConcurrentDictionary<string, List<SearchSuggestion>>();
        private readonly DataContext context;

        private const int MAX_SUGGESTIONS = 3;
        private const int WEIGHT_MINERAL_NAME_START = 3;
        private const int WEIGHT_MINERAL_WORD_START = 2;
        private const int WEIGHT_MINERAL_WORD_MIDDLE = 1;

        public SearchService(DataContext context)
        {
            this.context = context;
        }

        public List<SearchSuggestion> GetSearchSuggestions(string searchTerm)
        {
            if (searchTerm == null) searchTerm = "";
            searchTerm = ReplaceSpecialLetters(searchTerm.ToLower());
            if (cache.ContainsKey(searchTerm))
            {
                return cache[searchTerm];
            }

            var possibleResults = new List<SearchSuggestion>();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                possibleResults.AddRange(SearchMinerals(searchTerm));
            }

            cache[searchTerm] = possibleResults;

            return possibleResults;
        }

        private List<SearchSuggestion> SearchMinerals(string searchTerm)
        {
            var minerals = context.Components
                .Where(c => c.ImageUrl != null && c.Searchable)
                .Include(c => c.ComponentFamily).ToList();

            var possibleMatches = new List<Tuple<Component, int>>();
            foreach (var mineral in minerals)
            {
                var name = ReplaceSpecialLetters(mineral.Name.ToLower());
                if (name.Contains(searchTerm))
                {
                    int score;
                    if (name.IndexOf(searchTerm) == 0) score = WEIGHT_MINERAL_NAME_START;
                    else if (name[name.IndexOf(searchTerm) - 1] == ' ') score = WEIGHT_MINERAL_WORD_START;
                    else score = WEIGHT_MINERAL_WORD_MIDDLE;

                    possibleMatches.Add(new Tuple<Component, int>(mineral, score));
                }
            }

            return possibleMatches
                .OrderByDescending(m => m.Item2)
                .ThenBy(m => m.Item1.Name.Length)
                .Take(MAX_SUGGESTIONS).Select(m => 
            new SearchSuggestion
            {
                Title = HighlightSearchTermInText(m.Item1.Name, searchTerm),
                Score = m.Item2,
                Type = SearchSuggestionTypeEnum.Mineral,
                ImageUrl = m.Item1.ImageUrl,
                Description = m.Item1.ComponentFamily?.ShortDescription,
                SearchLinkUrl = ReplaceSpecialLetters(m.Item1.Name.ToLower()).Replace(" ", "-")
            }).ToList();
        }

        private string ReplaceSpecialLetters(string input)
        {
            return input.Replace('á', 'a')
                        .Replace('é', 'e')
                        .Replace('í', 'i')
                        .Replace('ó', 'o')
                        .Replace('ö', 'o')
                        .Replace('ő', 'o')
                        .Replace('ú', 'u')
                        .Replace('ü', 'u')
                        .Replace('ű', 'u');
        }

        private string HighlightSearchTermInText(string text, string searchTerm)
        {
            var simplifiedText = ReplaceSpecialLetters(text.ToLower());
            var indexes = new List<int>();

            int startIndex = simplifiedText.IndexOf(searchTerm);
            while (startIndex >= 0)
            {
                indexes.Add(startIndex);
                startIndex = simplifiedText.IndexOf(searchTerm, startIndex + searchTerm.Length);
            } 

            foreach (var index in indexes.OrderByDescending(i => i))
            {
                text = text.Substring(0, index) + "<mark>" + text.Substring(index, searchTerm.Length) + "</mark>" + text.Substring(index + searchTerm.Length);
            }

            return text;
        }

        public ConcurrentDictionary<string, List<SearchSuggestion>> GetCacheInformation()
        {
            return cache;
        }
    }
}
