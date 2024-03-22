using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICustomerService customerService;
        private readonly IPromotionService promotionService;

        private int customerId;
        public int CustomerId
        {
            get
            {
                return 0; // todo
                if (customerId <= 0)
                {
                    var customerCookieId = Request.Cookies["Id"];
                    if (customerCookieId == null)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Expires = new DateTimeOffset(Helper.Now.AddYears(10))
                        };
                        customerCookieId = Guid.NewGuid().ToString();
                        Response.Cookies.Append("Id", customerCookieId, cookieOptions);
                    }
                    CookieId = customerCookieId;
                    customerId = customerService.GetOrCreateCustomer(customerCookieId, UserHelper.GetReferrer(Request)).Id;
                }
                return customerId;
            }
        }

        public string CookieId { get; private set; }
        // todo
        protected BaseController(/*IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService*/)
        {
            /*this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));*/
        }

        protected BaseController(IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // todo
            base.OnActionExecuted(context);
            //ViewData["IsProd"] = configuration.GetValue<bool>("Settings:IsProd");
            //ViewData["Website"] = configuration.GetValue<string>("Settings:Website");
            //ViewData["AddFbPixel"] = configuration.GetValue<bool>("Settings:AddFbPixel");
            //ViewData["AddTagManager"] = configuration.GetValue<bool>("Settings:AddTagManager");
            //ViewData["IsChristmasMode"] = configuration.GetValue<bool>("Settings:IsChristmasMode");
            //ViewData["CustomerId"] = CustomerId;
            //ViewBag.FbExternalId = CookieId;
            //ViewData["IsFreeShippingPromotion"] = promotionService.IsPromotionActive(PromotionEnum.FreeShipping);
            //if ((bool)ViewData["IsFreeShippingPromotion"])
            //{
            //    var promotion = promotionService.GetCurrentOrNextPromotion(PromotionEnum.FreeShipping);
            //    ViewData["IsFreeShippingPromotionEndDate"] = promotion.EndDate;
            //}
        }
    }
}
