using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.RelatedProducts
{
    public class RelatedProductsService : IRelatedProductsService
    {
        private readonly IProductService productService;
        private readonly DataContext context;

        public RelatedProductsService(IProductService productService, DataContext context)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<ProductListItemViewModel> GetRelatedProducts(int productId, ProductTypeEnum productType)
        {
            const int maxCount = 10;

            var product = productService.GetBracelet(productId);
            if (product == null) return new List<ProductListItemViewModel>();
            return productService.GetActiveProducts(p => p.CategoryId == product.CategoryId &&
                                p.Id != productId &&
                                p.State == ProductStateEnum.Active)
                    .Where(p => !p.SoldOut)
                    .Take(maxCount)
                    .Select(p => new ProductListItemViewModel(p))
                    .ToList();

        }
    }
}
