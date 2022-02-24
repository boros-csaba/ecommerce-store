using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public interface IFeedModel
    {
        public string GetHeader();
        public string GetContent();
    }
}
