using elenora.BusinessModels;
using elenora.Features.Inventory;
using elenora.Features.ProductFeeds;
using elenora.Features.ProductList.Admin;
using elenora.Features.ProductPricing;
using elenora.Features.QualityAuditing;
using elenora.Models;
using elenora.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList
{
    public class ProductListService : IProductListService
    {
        private readonly DataContext context;
        private readonly IProductPricingService productPricingService;
        private readonly IWishlistService wishlistService;
        private readonly IInventoryService inventoryService;
        private readonly IQualityAuditingService qualityAuditingService;

        private const int ITEMS_PER_PAGE = 2000;
        private const int CACHE_TIMEOUT_HOURS = 2;
        private static DateTime productCacheUpdated = DateTime.MinValue;
        private static readonly List<Bracelet> productCache = new List<Bracelet>();

        private static DateTime categoryCacheUpdated = DateTime.MinValue;
        private static readonly List<Category> categoryCache = new List<Category>();

        public ProductListService(IInventoryService inventoryService, IWishlistService wishlistService, IProductPricingService productPricingService, IQualityAuditingService qualityAuditingService, DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            this.qualityAuditingService = qualityAuditingService ?? throw new ArgumentNullException(nameof(qualityAuditingService));
        }

        public ProductList GetProductList(int customerId, ProductCategoryTypeEnum categoryType, string categoryName, int page)
        {
            var category = context.Categories.FirstOrDefault(c => c.IdString == categoryName.ToLower() && c.Type == categoryType);
            var wishlistedProductIds = wishlistService.GetWishlistProductIds(customerId);
            var allProducts = GetProducts(category.Id);
            var products = allProducts
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE);
            return new ProductList
            {
                Title = category.Name,
                Description = string.IsNullOrWhiteSpace(category.Description) ? category.Name : category.Description,
                Count = allProducts.Count,
                Items = products.Select(p => new ProductListItem
                {
                    ProductId = p.Id,
                    ProductIdString = p.IdString,
                    ProductName = p.Name,
                    Category = category.Name,
                    Price = p.Price.Price,
                    OriginalPrice = p.Price.OriginalPrice,
                    InWishlist = wishlistedProductIds.Contains(p.Id),
                    SoldOut = p.SoldOut
                }).ToList()
            };
        }

        public ProductCategoryModel GetProductCategory(int customerId, ProductCategoryTypeEnum categoryType)
        {
            UpdateCategoryCacheIfNeeded();
            var wishlistedProductIds = wishlistService.GetWishlistProductIds(customerId);

            var mainCategory = categoryCache.First(c => c.Id == (int)categoryType);
            var result = new ProductCategoryModel
            {
                Title = mainCategory.Name,
                Description = mainCategory.Description,
                IdString = mainCategory.IdString
            };
            var categories = categoryCache.Where(c => c.Type == categoryType);
            foreach (var category in categories)
            {
                var subcategoryModel = new ProductSubcategoryModel
                {
                    Title = category.Name,
                    Description = category.Description,
                    IdString = category.IdString,
                    Products = GetProducts(category.Id)
                        .OrderBy(p => p.ListOrder == 0).ThenBy(o => o.ListOrder).Take(4).Select(p => new ProductListItem
                    {
                        ProductId = p.Id,
                        ProductIdString = p.IdString,
                        ProductName = p.Name,
                        Category = category.Name,
                        Price = p.Price.Price,
                        OriginalPrice = p.Price.OriginalPrice,
                        InWishlist = wishlistedProductIds.Contains(p.Id),
                        SoldOut = p.SoldOut
                    }).ToList()
                };
                if (subcategoryModel.Products.Count >= 4)
                {
                    result.Subcategories.Add(subcategoryModel);
                }
            }
            return result;
        }

        public ProductCategoryModel GetProductsByMinerals(int customerId)
        {
            UpdateCategoryCacheIfNeeded();
            var wishlistedProductIds = wishlistService.GetWishlistProductIds(customerId);
            var mainCategory = categoryCache.First(c => c.IdString == "karkotok-asvanyok-szerint");
            var result = new ProductCategoryModel
            {
                Title = mainCategory.Name,
                Description = mainCategory.Description,
                IdString = mainCategory.IdString
            };
            var allProducts = GetAllProducts().Where(p =>
                    (p.CategoryId < 11 || p.CategoryId > 22) &&
                    !p.Name.Contains("férfi")).ToList();

            foreach (var product in allProducts)
            {
                foreach (var component in product.ProductComponents.Where(pc => pc.Component.ComponentFamilyId > 0 && !string.IsNullOrWhiteSpace(pc.Component.ComponentFamily.IdString)))
                {
                    var subcategory = result.Subcategories.FirstOrDefault(s => s.IdString == component.Component.ComponentFamily.IdString);
                    if (subcategory == null)
                    {
                        subcategory = new ProductSubcategoryModel
                        {
                            Title = component.Component.ComponentFamily.Name,
                            Description = component.Component.ComponentFamily.Description,
                            IdString = component.Component.ComponentFamily.IdString
                        };
                        result.Subcategories.Add(subcategory);
                    }
                    subcategory.Products.Add(new ProductListItem
                    {
                        ProductId = product.Id,
                        ProductIdString = product.IdString,
                        ProductName = product.Name,
                        Category = product.Category.Name,
                        Price = product.Price.Price,
                        OriginalPrice = product.Price.OriginalPrice,
                        InWishlist = wishlistedProductIds.Contains(product.Id),
                        SoldOut = product.SoldOut,
                        SortingWeight = component.Count,
                        CategoryType = product.Category.Type
                    });
                }
            }
            foreach (var subcategory in result.Subcategories)
            {
                subcategory.Products = subcategory.Products
                    .OrderByDescending(p => p.SortingWeight)
                    .Where(p => p.CategoryType == ProductCategoryTypeEnum.WomansBracelets || p.CategoryType == ProductCategoryTypeEnum.MansBracelets)
                    .Take(4).ToList();
            }
            result.Subcategories.RemoveAll(r => r.Products.Count < 4);
            result.Subcategories = result.Subcategories.OrderBy(s => s.Title).ToList();
            return result;
        }

        public ProductList GetProductList(int customerId, string mineral, int page)
        {
            var componentFamily = context.ComponentFamilies.FirstOrDefault(c => c.IdString == mineral.ToLower());
            var component = context.Components.FirstOrDefault(c => c.IdString == mineral.ToLower());
            var wishlistedProductIds = wishlistService.GetWishlistProductIds(customerId);
            var allProducts = GetProductListByMineral(mineral);
            var products = allProducts
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE);
            return new ProductList
            {
                Title = componentFamily?.Name ?? component?.Name,
                Description = string.IsNullOrWhiteSpace(componentFamily?.Description) ? componentFamily?.Name : componentFamily?.Description,
                Count = allProducts.Count,
                Items = products.Select(p => new ProductListItem
                {
                    ProductId = p.Id,
                    ProductIdString = p.IdString,
                    ProductName = p.Name,
                    Category = componentFamily?.Name ?? component?.Name,
                    Price = p.Price.Price,
                    OriginalPrice = p.Price.OriginalPrice,
                    InWishlist = wishlistedProductIds.Contains(p.Id),
                    SoldOut = p.SoldOut,
                    SortingWeight = p.ProductComponents.Where(pc => (componentFamily != null && pc.Component.ComponentFamilyId == componentFamily?.Id) || (component != null && pc.ComponentId == component?.Id)).Sum(pc => pc.Count)
                }).OrderByDescending(p => p.SortingWeight).ToList()
            };
        }

        private List<Bracelet> GetProductListByMineral(string mineral)
        {
            if (string.IsNullOrWhiteSpace(mineral))
                return new List<Bracelet>();

            var componentFamily = context.ComponentFamilies.FirstOrDefault(c => c.IdString == mineral.ToLower());
            var component = context.Components.FirstOrDefault(c => c.IdString == mineral.ToLower());
            var allProducts = GetAllProducts().Where(p =>
                p.ProductComponents.Any(pc => (componentFamily != null && pc.Component.ComponentFamilyId == componentFamily.Id) || (component != null && pc.ComponentId == component.Id)) &&
                (p.CategoryId < 11 || p.CategoryId > 22) &&
                !p.Name.Contains("férfi")).ToList();
            return allProducts;
        }

        private List<Bracelet> GetProducts(int categoryId)
        {
            UpdateProductCacheIfNeeded();
            return productCache.Where(p => p.CategoryId == categoryId).ToList();
        }

        private List<Bracelet> GetAllProducts()
        {
            UpdateProductCacheIfNeeded();
            return productCache;
        }

        private void UpdateProductCacheIfNeeded(bool forceUpdate = false)
        {
            if (productCacheUpdated.AddHours(CACHE_TIMEOUT_HOURS) < Helper.Now || forceUpdate)
            {
                var products = context.Products
                    .Include(p => p.ProductDiscounts)
                    .Include(p => p.Category)
                    .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component).ThenInclude(c => c.ComponentFamily)
                    .Where(p => p.State == ProductStateEnum.Active)
                    .OrderByDescending(p => p.ProductComponents.Average(pc => pc.Component.ImagesQuality))
                    .ToList();
                foreach (var product in products)
                {
                    product.Price = productPricingService.GetProductPrice(product);
                    product.SoldOut = inventoryService.IsSoldOut(product);
                }
                productCache.Clear();
                productCache.AddRange(products);
                productCacheUpdated = Helper.Now;
            }
        }

        private void UpdateCategoryCacheIfNeeded()
        {
            if (categoryCacheUpdated.AddHours(CACHE_TIMEOUT_HOURS) < Helper.Now)
            {
                categoryCache.Clear();
                categoryCache.AddRange(context.Categories.ToList());
                categoryCacheUpdated = Helper.Now;
            }
        }

        private static List<ProductCategoryNode> cachedReport;
        private static DateTime reportCacheUpdated = DateTime.MinValue;
        public List<ProductCategoryNode> GetProductsAdminReport()
        {
            if (reportCacheUpdated.AddHours(CACHE_TIMEOUT_HOURS) < Helper.Now)
            {
                UpdateProductCacheIfNeeded(true);
                var result = new List<ProductCategoryNode>
                {
                    GetProductsByCategoryNode(),
                    GetProductFeedsNode(),
                    GetProductQualityIssuesNode()
                };
                cachedReport = result;
                reportCacheUpdated = Helper.Now;
            }
            return cachedReport;
        }

        private void CollectProductsForNode(int categoryId, ProductCategoryNode node, ProductCategoryNode parentNode)
        {
            var products = context.Products
                .Include(p => p.ProductDiscounts)
                .Include(p => p.ProductComponents).ThenInclude(c => c.Component).ThenInclude(c => c.ComponentSuppliers)
                .Include(p => p.Category)
                .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component).ThenInclude(c => c.ComponentFamily)
                .Where(p => p.CategoryId == categoryId).ToList();
            CollectProductsForNode(products, node, parentNode);
        }

        private void CollectProductsForNode(string componentFamilyIdString, ProductCategoryNode node, ProductCategoryNode parentNode)
        {
            var products = GetProductListByMineral(componentFamilyIdString);
            CollectProductsForNode(products, node, parentNode);
        }

        private void CollectProductsForNode(List<Bracelet> products, ProductCategoryNode node, ProductCategoryNode parentNode)
        {
            foreach (var product in products)
            {
                product.SoldOut = inventoryService.IsSoldOut(product);
                var productModel = GetProductNode(product);

                if (product.State == ProductStateEnum.Active)
                {
                    node.ActiveProductsCount++;
                }
                else
                {
                    node.PendingProductsCount++;
                }
                if (product.SoldOut)
                {
                    node.SoldOutProductsCount++;
                }
                node.Children.Add(productModel);
            }
            if (parentNode != null)
            {
                AddNodeToParent(node, parentNode);
            }
        }

        private void AddNodeToParent(ProductCategoryNode childNode, ProductCategoryNode parentNode, bool addCountToParent = true)
        {
            parentNode.Children.Add(childNode);
            if (addCountToParent)
            {
                parentNode.ActiveProductsCount += childNode.ActiveProductsCount;
                parentNode.SoldOutProductsCount += childNode.SoldOutProductsCount;
                parentNode.PendingProductsCount += childNode.PendingProductsCount;
            }
        }

        private ProductCategoryNode GetProductsByCategoryNode()
        {
            var parent = new ProductCategoryNode { Name = "Kategóriák szerint" };

            var categories = context.Categories
                .Where(c => c.Type == ProductCategoryTypeEnum.MainCategory)
                .OrderBy(c => c.Id).ToList();
            foreach (var category in categories)
            {
                var model = new ProductCategoryNode
                {
                    Name = category.Name
                };
                var subcategories = context.Categories
                    .Where(c => (int)c.Type == category.Id)
                    .OrderBy(c => c.Id).ToList();
                foreach (var subcategory in subcategories)
                {
                    var submodel = new ProductCategoryNode
                    {
                        Name = subcategory.Name
                    };
                    CollectProductsForNode(subcategory.Id, submodel, model);
                }

                const int mineralsCategoryId = 41;
                if (category.Id == mineralsCategoryId)
                {
                    var componentFamilies = context.ComponentFamilies.OrderBy(c => c.Name).ToList();
                    foreach (var componentFamily in componentFamilies)
                    {
                        var componentModel = new ProductCategoryNode
                        {
                            Name = componentFamily.Name
                        };
                        CollectProductsForNode(componentFamily.IdString, componentModel, model);
                    }
                    AddNodeToParent(model, parent, false);
                }
                else
                {
                    CollectProductsForNode(category.Id, model, null);
                    AddNodeToParent(model, parent);
                }
            }

            return parent;
        }

        private ProductCategoryNode GetProductFeedsNode()
        {
            var parent = new ProductCategoryNode { Name = "Feedek szerint" };

            AddNodeToParent(CollectProductsForFeedNode("Árukereső", ProductFeedTypeEnum.Arukereso), parent, false);
            AddNodeToParent(CollectProductsForFeedNode("Google shopping", ProductFeedTypeEnum.GoogleShopping), parent, false);
            AddNodeToParent(CollectProductsForFeedNode("Pinterest", ProductFeedTypeEnum.Pinterest), parent, false);

            return parent;
        }

        private ProductCategoryNode GetProductQualityIssuesNode()
        {
            var parent = new ProductCategoryNode { Name = "Termék hibák" };
            var products = context.Products
                .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component).ThenInclude(c => c.ComponentFamily)
                .Include(p => p.ProductFeedVisibilities)
                .ToList();
            var productIssues = qualityAuditingService.GetProductQualityIssues(products);
            foreach (var issue in productIssues.Keys)
            {
                var issueCategory = new ProductCategoryNode { Name = issue };
                CollectProductsForNode(productIssues[issue].Select(i => i.Product).ToList(), issueCategory, parent);
            }
            return parent;
        }

        private ProductCategoryNode CollectProductsForFeedNode(string feedName, ProductFeedTypeEnum productFeedType)
        {
            var all = new ProductCategoryNode { Name = feedName };
            var included = new ProductCategoryNode { Name = "Feedben" };
            var excluded = new ProductCategoryNode { Name = "Kihagyva" };

            var products = context.Products
                .Include(p => p.ProductDiscounts)
                .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component).ThenInclude(c => c.ComponentFamily)
                .Include(p => p.ProductFeedVisibilities)
                .ToList();

            foreach (var product in products)
            {
                product.SoldOut = inventoryService.IsSoldOut(product);
                var productNode = GetProductNode(product);
                productNode.Feed = (int)productFeedType;

                var node = excluded;
                var visibility = product.ProductFeedVisibilities.FirstOrDefault(v => v.FeedType == productFeedType);
                if (visibility != null && visibility.Visible)
                {
                    node = included;
                }

                if (product.State == ProductStateEnum.Active)
                {
                    node.ActiveProductsCount++;
                }
                else
                {
                    node.PendingProductsCount++;
                }
                if (product.SoldOut)
                {
                    node.SoldOutProductsCount++;
                }
                node.Children.Add(productNode);
            }
            AddNodeToParent(included, all);
            AddNodeToParent(excluded, all);
            return all;
        }

        private ProductCategoryNode GetProductNode(Bracelet product)
        {
            return new ProductCategoryNode
            {
                Name = product.Name,
                IdString = product.IdString,
                IsCategory = false,
                ButtonsVisible = true,
                CategoryName = GetCategoryFullName(product.CategoryId),
                Price = product.BasePrice,
                Components = string.Join("<br />", product.ProductComponents.OrderByDescending(pc => pc.Count).Select(pc => pc.ShowOnProduct ? $"<b>{pc.Component.Name} x {pc.Count}</b>" : $"{pc.Component.Name} x {pc.Count}")),
                MarginsMinTotal = productPricingService.GetMarginsInformation(product).MinTotal,
                MarginsMaxTotal = productPricingService.GetMarginsInformation(product).MaxTotal,
                MarginsDescription = productPricingService.GetMarginsInformation(product).Description,
                MarginsMissingInformation = productPricingService.GetMarginsInformation(product).MissingInformation,
                Status = product.State == ProductStateEnum.Active ? product.SoldOut ? "Elfogyott" : "" : "Függő"
            };
        }

        private string GetCategoryFullName(int categoryId)
        {
            UpdateCategoryCacheIfNeeded();
            var category = categoryCache.First(c => c.Id == categoryId);
            var parent = categoryCache.FirstOrDefault(c => c.Id == (int)category.Type);
            if (parent != null) return $"{parent.Name} / { category.Name}";
            return category.Name;
        }
    }
}
