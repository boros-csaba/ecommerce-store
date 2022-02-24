using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class SearchController: BaseController
    {
        private readonly IActionLogService actionLogService;
        private readonly IProductService productService;
        private readonly IWishlistService wishlistService;
        private readonly ISearchService searchService;

        public SearchController(IActionLogService actionLogService, IProductService productService, IWishlistService wishlistService, ISearchService searchService, IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService): base(configuration, customerService, promotionService)
        {
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
            this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        [Route("/kereses/{searchTerm}")]
        public IActionResult Search(string searchTerm)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.Search, null, searchTerm);

            var results = productService.Search(searchTerm);
            var wishlist = wishlistService.GetWishlistProductIds(CustomerId);
            var model = new SearchListViewModel
            {
                Products = results.Select(p => new ProductListItemViewModel(p)).ToList(),
                SearchTerm = searchTerm.Replace("-", " ")
            };
            model.Products.ForEach(p => p.InWishlist = wishlist.Contains(p.Id));
            return View(model);
        }

        [Route("/kereses-ajanlat/{searchTerm}")]
        public IActionResult SearchSuggestion(string searchTerm)
        {
            var suggestions = searchService.GetSearchSuggestions(searchTerm);
            return Json(suggestions);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/search-cache")]
        public IActionResult SearchCacheCheck()
        {
            return Json(searchService.GetCacheInformation());
        }
    }
}
