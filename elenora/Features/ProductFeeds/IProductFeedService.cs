using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public interface IProductFeedService
    {
        string GetFeedData(string target);
        string GetFeedJson(string target);
    }
}
