using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IArticleService
    {
        public Article GetArticle(string id);
        public List<Article> GetArticles();
    }
}
