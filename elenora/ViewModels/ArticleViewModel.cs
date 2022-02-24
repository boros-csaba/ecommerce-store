using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<ComponentViewModel> Beads { get; set; }

        public ArticleViewModel(Article article)
        {
            Id = article.Id;
            IdString = article.IdString;
            Title = article.Title;
            Content = article.Content;
        }
    }
}
