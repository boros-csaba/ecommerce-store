using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class ArticleService : IArticleService
    {
        private readonly DataContext context;

        public ArticleService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Article GetArticle(string id)
        {
            return context.Articles.FirstOrDefault(a => a.IdString == id.ToLower());
        }

        public List<Article> GetArticles()
        {
            return context.Articles.OrderByDescending(a => a.Id).ToList();
        }
    }
}
