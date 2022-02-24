using elenora.BusinessModels;
using elenora.Features.Inventory;
using elenora.Features.ProductPricing;
using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace elenora.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductPricingService productPricingService;
        private readonly IInventoryService inventoryService;
        private readonly IEmailService emailService;
        private readonly DataContext context;

        public ProductService(IInventoryService inventoryService, IEmailService emailService, IProductPricingService productPricingService, DataContext context)
        {
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.context = context ?? throw new ArgumentException(nameof(context));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        public List<Bracelet> GetFeaturedProducts(int? limit = null)
        {
            var products = GetActiveProducts(p => p.Featured && p.State == ProductStateEnum.Active)
                .OrderBy(p => p.ListOrder);
            if (limit != null) return products.Take(limit.Value).ToList();
            return products.ToList();
        }

        public List<Bracelet> GetDiscountedProducts()
        {
            var productIds = productPricingService.GetDiscountedProductIds();
            return GetActiveProducts(b => productIds.Contains(b.Id));
        }

        public Bracelet GetBracelet(string idString)
        {
            return GetActiveProducts(p => p.IdString == idString.ToLower()).FirstOrDefault();
        }

        public Bracelet GetBracelet(int id)
        {
            return GetAllProducts(p => p.Id == id, false).FirstOrDefault();
        }

        public List<Bracelet> GetProducts(List<string> productIds)
        {
            return GetActiveProducts(p => productIds.Contains(p.IdString));
        }

        public Category GetCategory(string categoryId)
        {
            return context.Categories.FirstOrDefault(c => c.IdString == categoryId.ToLower());
        }

        public List<Bracelet> GetProductsByCategory(Category category, int? limit = null)
        {
            var products =GetActiveProducts(p => p.CategoryId == category.Id && p.State == ProductStateEnum.Active)
                .OrderBy(p => p.ListOrder).ThenByDescending(p => p.Id);
            if (limit != null) return products.Take(limit.Value).ToList();
            return products.ToList();
        }

        public List<Bracelet> Search(string searchTerm)
        {
            var words = searchTerm.Split("-", StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().ToLower()).Where(w => w.Length > 2);
            var result = new List<ProductWithSearchScore>();
            foreach (var word in words)
            {
                var partialResults = GetActiveProducts(p => p.State == ProductStateEnum.Active &&
                                p.ProductType != ProductTypeEnum.ExtraBracelet &&
                                (p.Name.ToLower().Contains(word) ||
                                p.ProductComponents.Any(c => c.Component.Name.ToLower().Contains(word)) ||
                                p.ProductComponents.Any(c => c.Component.ComponentFamily != null && c.Component.ComponentFamily.Name.ToLower().Contains(word)) ||
                                p.ProductComponents.Any(c => c.Component.ComponentFamily != null && c.Component.ComponentFamily.Description.ToLower().Contains(word))))
                    .Select(p => new ProductWithSearchScore { Product = p, Score = 0 })
                    .ToList();
                result.AddRange(partialResults.Where(p => !result.Any(pp => p.Product.Id == pp.Product.Id)));
            }
            foreach (var item in result)
            {
                foreach (var word in words)
                {
                    if (item.Product.Name.ToLower().Contains(word))
                    {
                        item.Score += 3;
                    }
                    if (item.Product.ProductComponents.Any(c => c.Component.ComponentFamily != null && c.Component.Name.ToLower().Contains(word)) ||
                        item.Product.ProductComponents.Any(c => c.Component.ComponentFamily != null && c.Component.ComponentFamily.Name.ToLower().Contains(word)))
                    {
                        item.Score += 2;
                    }
                    if (item.Product.ProductComponents.Any(c => c.Component.ComponentFamily != null && c.Component.ComponentFamily.Description.ToLower().Contains(word)))
                    {
                        item.Score += 1;
                    }
                }
            }
            return result.OrderByDescending(o => o.Score).Select(o => o.Product).ToList();
        }

        public List<Component> GetProductComponents(int productId, bool onlyVisible)
        {
            return context.ProductComponents
                .Where(pc => pc.ShowOnProduct || !onlyVisible)
                .Include(pc => pc.Component)
                .ThenInclude(c => c.ComponentFamily)
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.Component).ToList();
        }

        public List<Component> GetComponents(List<string> componentNames)
        {
            return context.Components
                .Include(c => c.ComponentFamily)
                .Where(c => componentNames.Contains(c.Name)).ToList();
        }

        public List<ComponentFamily> GetComponentsCategories()
        {
            return context.ComponentFamilies
                .Include(f => f.Components)
                .ToList();
        }

        public List<Component> GetBraceletDesignerComponents()
        {
            return context.Components
                .Include(c => c.ComponentFamily)
                .Include(c => c.ComponentImages)
                .Include(c => c.BeadComplementaryProducts).ThenInclude(e => e.ComplementaryProduct)
                .Where(c => c.SelectabeInBraceletDesigner).ToList();
        }

        public List<Component> GetLetters(bool white)
        {
            if (white) return context.Components.Where(c => c.Name.StartsWith("Fehér") && c.Name.Length == 7).ToList();
            else return context.Components.Where(c => c.Name.StartsWith("Fekete") && c.Name.Length == 8).ToList();
        }

        private class ProductWithSearchScore
        {
            public Bracelet Product { get; set; }
            public int Score { get; set; }
        }

        public List<Bracelet> GetActiveProducts(Expression<Func<Bracelet, bool>> filter)
        {
            return GetAllProducts(filter, true);
        }

        public void LogProductView(int productId)
        {
            var product = GetAllProducts(p => p.Id == productId, false).FirstOrDefault();
            if (product == null) return;
            var discount = product.ProductDiscounts.Where(d => d.StartDate < Helper.Now && Helper.Now < d.EndDate).FirstOrDefault();
            if (discount != null)
            {
                discount.Views++;
            }
            var productStatistics = GetCurrentStatistics(productId);
            productStatistics.Views++;
            context.SaveChanges();
        }

        public void LogProductAddToCart(int productId)
        {
            var product = GetAllProducts(p => p.Id == productId, false).FirstOrDefault();
            if (product == null) return;
            var discount = product.ProductDiscounts.Where(d => d.StartDate < Helper.Now && Helper.Now < d.EndDate).FirstOrDefault();
            if (discount != null)
            {
                discount.AddToCarts++;
            }
            var productStatistics = GetCurrentStatistics(productId);
            productStatistics.AddToCarts++;
            context.SaveChanges();
        }

        public void LogProductPurchase(int productId)
        {
            var product = GetAllProducts(p => p.Id == productId, false).FirstOrDefault();
            if (product == null) return;
            var discount = product.ProductDiscounts.Where(d => d.StartDate < Helper.Now && Helper.Now < d.EndDate).FirstOrDefault();
            if (discount != null)
            {
                discount.Purchases++;
            }
            var productStatistics = GetCurrentStatistics(productId);
            productStatistics.Purchases++;
            context.SaveChanges();
        }

        private ProductStatistics GetCurrentStatistics(int productId)
        {
            var date = Helper.Now.AddDays(-1 * (int)Helper.Now.DayOfWeek + 1).Date;
            var result = context.ProductStatistics.FirstOrDefault(s =>
                s.ProductId == productId && s.Date == date);
            if (result == null)
            {
                result = new ProductStatistics
                {
                    ProductId = productId,
                    Date = date
                };
                context.ProductStatistics.Add(result);
            }
            return result;
        }

        private List<Bracelet> GetAllProducts(Expression<Func<Bracelet, bool>> filter, bool onlyActive)
        {
            var result = context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDiscounts)
                .Include(p => p.ProductFeedVisibilities)
                .Include(p => p.ProductComponents).ThenInclude(p => p.Component).ThenInclude(p => p.ComponentFamily)
                .Where(p => p.State == ProductStateEnum.Active || !onlyActive)
                .Where(filter)
                .ToList();

            foreach (var product in result)
            {
                product.Price = productPricingService.GetProductPrice(product);
                product.SoldOut = inventoryService.IsSoldOut(product);
            }
            if (result.Any(p => p.Price.Price <= 0))
            {
                var message = string.Join(", ", result.Where(p => p.Price.Price <= 0).Select(p => $"{p.Name} - {p.Id} - {p.IdString}"));
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Hibás termék 0 árral!", message, message, "product-error");
            }
            result.RemoveAll(p => p.Price.Price <= 0);
            return result;
        }
    }
}
