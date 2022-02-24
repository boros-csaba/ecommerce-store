using elenora.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.RelatedProducts
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly IRelatedProductsService relatedProductsService;

        public RelatedProductsViewComponent(IRelatedProductsService relatedProductsService)
        {
            this.relatedProductsService = relatedProductsService ?? throw new ArgumentNullException(nameof(relatedProductsService));
        }

        public IViewComponentResult Invoke(int productId, ProductTypeEnum productType)
        {
            var model = relatedProductsService.GetRelatedProducts(productId, productType);
            return View(model);
        }
    }
}
