using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace elenora.Controllers
{
    public class LandingPageController : BaseController
    {
        private readonly IActionLogService actionLogService;
        private readonly IProductService productService;
        private readonly IWebHostEnvironment environment;

        public LandingPageController(IConfiguration configuration, ICustomerService customerService, IActionLogService actionLogService, IProductService productService, IPromotionService promotionService, IWebHostEnvironment environment) : base(configuration, customerService, promotionService)
        {
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.environment = environment;
        }

        [Route("/ajandek-paros-karkotok-{version}")]
        public IActionResult GiftCouplesBracelets(int version)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewLandingPage, null, "GiftCouplesBracelets-" + version);
            var model = new LandingPageViewModel
            {
                Products = productService.GetProducts(new List<string>
                {
                    "our-world-turkiz-regalit-achat",
                    "crowns-szurke-kepjaspis-howlit-cirkonia",
                    "yin-and-yang-howlit-lavako",
                    "savvy-love-rozsaszin-jade-achat",
                    "galaxy-lovers-lila-kek-regalit-achat",
                    "set-in-stone-szurke-jaspis-achat",
                    "love-flame-voros-jaspis-achat",
                    "every-breath-kek-jade-fekete-achat",
                    "black-and-white-howlit-achat"
                }).Select(p => new ProductViewModel(p, environment.WebRootPath)).ToList()
            };
            return View(model);
        }
    }
}