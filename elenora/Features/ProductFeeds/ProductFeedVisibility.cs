using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class ProductFeedVisibility
    {
        public int Id { get; set; }
        public ProductFeedTypeEnum FeedType { get; set; }
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public bool Visible { get; set; }
    }
}
