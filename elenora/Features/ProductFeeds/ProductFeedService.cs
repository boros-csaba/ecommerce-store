using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class ProductFeedService : IProductFeedService
    {
        private readonly IProductService productService;
        private readonly DataContext context;

        public ProductFeedService(IProductService productService, DataContext context)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetFeedData(string target)
        {
            var feedData = GetFeedDataItems(target);
            if (!feedData.Any()) return string.Empty;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(feedData.First().GetHeader());
            foreach (var item in feedData)
            {
                stringBuilder.AppendLine(item.GetContent());
            }
            return stringBuilder.ToString();
        }

        public string GetFeedJson(string target)
        {
            var feedData = GetFeedDataItems(target);
            return GetFeedType(target) switch
            {
                ProductFeedTypeEnum.Facebook => JsonSerializer.Serialize(feedData.Select(d => d as FacebookFeedModel)),
                ProductFeedTypeEnum.Arukereso => JsonSerializer.Serialize(feedData.Select(d => d as ArukeresoFeedModel)),
                ProductFeedTypeEnum.GoogleShopping => JsonSerializer.Serialize(feedData.Select(d => d as GoogleShoppingFeedModel)),
                ProductFeedTypeEnum.Pinterest => JsonSerializer.Serialize(feedData.Select(d => d as PinterestFeedModel)),
                _ => string.Empty,
            };
        }

        private List<IFeedModel> GetFeedDataItems(string target)
        {
            var feedType = GetFeedType(target);
            var result = new List<IFeedModel>();

            List<Bracelet> bracelets;
            if (feedType == ProductFeedTypeEnum.Facebook)
            {
                bracelets = productService.GetActiveProducts(p =>
                    p.State == ProductStateEnum.Active &&
                    (p.ProductType == ProductTypeEnum.Bracelet || p.ProductType == ProductTypeEnum.BraceletPair));
            }
            else
            {
                bracelets = productService.GetActiveProducts(p =>
                    p.State == ProductStateEnum.Active &&
                    p.ProductFeedVisibilities.Any(v => v.FeedType == feedType && v.Visible));
            }

            if (feedType == ProductFeedTypeEnum.Facebook)
            {
                result.AddRange(bracelets.Select(b => new FacebookFeedModel(b)).ToList());
                result.Add(new FacebookFeedModel("custom-bracelet"));
            }
            else if (feedType == ProductFeedTypeEnum.Arukereso)
            {
                result.AddRange(bracelets.Select(b => new ArukeresoFeedModel(b)).ToList());
            }
            else if (feedType == ProductFeedTypeEnum.GoogleShopping)
            {
                result.AddRange(bracelets.Select(b => new GoogleShoppingFeedModel(b)).ToList());
            }
            else if (feedType == ProductFeedTypeEnum.Pinterest)
            {
                result.AddRange(bracelets.Select(b => new PinterestFeedModel(b)).ToList());
            }
            return result;
        }

        private ProductFeedTypeEnum GetFeedType(string target)
        {
            if (target.ToLower() == "facebook") return ProductFeedTypeEnum.Facebook;
            if (target.ToLower() == "arukereso") return ProductFeedTypeEnum.Arukereso;
            if (target.ToLower() == "google-shopping") return ProductFeedTypeEnum.GoogleShopping;
            if (target.ToLower() == "pinterest") return ProductFeedTypeEnum.Pinterest;
            throw new ArgumentException($"Unknown feed: {target}");
        }
    }
}
