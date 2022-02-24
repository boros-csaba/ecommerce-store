using elenora.Controllers;
using elenora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace elenora.Features.ProductList
{
    public class ProductListController : BaseController
    {
        private readonly IProductListService productListService;

        public ProductListController(IProductListService productListService, IPromotionService promotionService, ICustomerService customerService, IConfiguration configuration) : base(configuration, customerService, promotionService)
        {
            this.productListService = productListService ?? throw new ArgumentNullException(nameof(productListService));
        }
        
        [Route("/noi-karkotok")]
        public IActionResult WomansCategory()
        {
            var model = productListService.GetProductCategory(CustomerId, ProductCategoryTypeEnum.WomansBracelets);
            return View("GeneralCategory", model);
        }

        [Route("/noi-karkotok/{subcategory}")]
        [Route("/noi-karkotok/{subcategory}/{page:int}")]
        public IActionResult WomansSubcategory(string subcategory, int page = 1)
        {
            var model = productListService.GetProductList(CustomerId, ProductCategoryTypeEnum.WomansBracelets, subcategory, page);
            return View("GeneralSubcategory", model);
        }

        [Route("/ferfi-karkotok")]
        public IActionResult MansCategory()
        {
            var model = productListService.GetProductCategory(CustomerId, ProductCategoryTypeEnum.MansBracelets);
            return View("GeneralCategory", model);
        }

        [Route("/ferfi-karkotok/{subcategory}")]
        [Route("/ferfi-karkotok/{subcategory}/{page:int}")]
        public IActionResult MansSubcategory(string subcategory, int page = 1)
        {
            var model = productListService.GetProductList(CustomerId, ProductCategoryTypeEnum.MansBracelets, subcategory, page);
            return View("GeneralSubcategory", model);
        }

        [Route("/paros-karkotok")]
        [Route("/paros-karkotok/{page:int}")]
        public IActionResult PairCategory(int page = 1)
        {
            var model = productListService.GetProductList(CustomerId, ProductCategoryTypeEnum.MainCategory, "paros-karkotok", page);
            return View("GeneralSubcategory", model);
        }

        [Route("/karkotok-asvanyok-szerint")]
        public IActionResult BraceletsByMinerals()
        {
            var model = productListService.GetProductsByMinerals(CustomerId);
            return View("GeneralCategory", model);
        }

        [Route("/karkotok-asvanyok-szerint/{mineral}")]
        [Route("/karkotok-asvanyok-szerint/{mineral}/{page:int}")]
        public IActionResult MineralSubcategory(string mineral, int page = 1)
        {
            var model = productListService.GetProductList(CustomerId, mineral, page);
            return View("GeneralSubcategory", model);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/products")]
        public IActionResult AdminReport()
        {
            var result = productListService.GetProductsAdminReport();
            return Ok(result);
        }
    }
}
