using elenora.Controllers;
using elenora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    public class ProductPricingController : BaseController
    {
        private readonly IProductPricingService productPricingService;
        private readonly DataContext context;

        public ProductPricingController(DataContext context, IProductPricingService productPricingService, IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/profit-report/{orderId}")]
        public ActionResult ProfitReport(int orderId)
        {
            return Ok(productPricingService.GetOrderCostInformation(orderId));
        }

    }
}
