using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class WishlistController: BaseController
    {
        private readonly IWishlistService wishlistService;
        private readonly IActionLogService actionLogService;

        public WishlistController(IConfiguration configuration, ICustomerService customerService, IWishlistService wishlistService, IActionLogService actionLogService, IPromotionService promotionService): base(configuration, customerService, promotionService)
        {
            this.wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
        }

        [HttpGet]
        [Route("/kivansag-lista")]
        public IActionResult Index()
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewWishList, null, null);

            var products = wishlistService.GetWishlistProducts(CustomerId).Select(p => new ProductListItemViewModel(p)).ToList();
            products.ForEach(p => p.InWishlist = true);
            return View(products);
        }

        [HttpPost]
        [Route("/kivansag-lista/hozzaadas")]
        public IActionResult AddToWishlist(int productId)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.AddToWishlist, productId, null);

            wishlistService.AddToWishlist(productId, CustomerId);
            return Ok();
        }

        [HttpPost]
        [Route("/kivansag-lista/torles")]
        public IActionResult RemoveFromWishlist(int productId)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.DeleteFromWishlist, productId, null);

            wishlistService.DeleteFromWishlist(productId, CustomerId);
            return Ok();
        }
    }
}
