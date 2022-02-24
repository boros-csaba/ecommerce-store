using elenora.BusinessModels;
using elenora.Features.ProductFeeds;
using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class ProductFeedsController : BaseController
    {
		private readonly DataContext context;
		private readonly IProductFeedService productFeedService;

        public ProductFeedsController(IProductFeedService productFeedService, DataContext context, IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
			this.productFeedService = productFeedService ?? throw new ArgumentNullException(nameof(productFeedService));
			this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("/product-feeds/{feed}")]
        public IActionResult GetFeedContent(string feed)
        {
			var validFeeds = new string[] { "facebook", "arukereso", "google-shopping", "pinterest" };
            var testFeed = "";
			if (validFeeds.Contains(feed.ToLower()))
            {
				var data = productFeedService.GetFeedData(feed);
				return Content(data);
			}
            if (feed.ToLower() == "glami")
            {
                testFeed = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SHOP>
	<SHOPITEM>
		<ITEM_ID>112</ITEM_ID>
		<PRODUCTNAME>Női Türkiz regalit karkötő - Timothea</PRODUCTNAME>
		<URL>https://www.elenora.hu/karkoto/timothea-turkiz-regalit?utm_source=glami&amp;utm_medium=cpc&amp;utm_campaign=timothea-turkiz-regalit</URL>
		<IMGURL>https://www.elenora.hu/images/products/timothea-turkiz-regalit/timothea-turkiz-regalit-2048.jpg</IMGURL>
		<PRICE_VAT>5790</PRICE_VAT>
		<CATEGORYTEXT>Glami.hu | Női ruházat és cipők | Női ékszerek és órák | Női karkötők és bokaláncok</CATEGORYTEXT>
		<MANUFACTURER>ELENORA</MANUFACTURER>		
		<PARAM>
			<PARAM_NAME>méret</PARAM_NAME>
			<VAL>M</VAL>
		</PARAM>
		<PARAM>
			<PARAM_NAME>Szín</PARAM_NAME>
			<VAL>türkiz</VAL>
		</PARAM>
		<DELIVERY_DATE>0</DELIVERY_DATE>
		<DELIVERY>
			<DELIVERY_ID>Magyar Posta</DELIVERY_ID>
			<DELIVERY_PRICE>0</DELIVERY_PRICE>
		</DELIVERY>
		<DELIVERY>
			<DELIVERY_ID>GLS</DELIVERY_ID>
			<DELIVERY_PRICE>1390</DELIVERY_PRICE>
			<DELIVERY_PRICE_COD>2080</DELIVERY_PRICE_COD>
		</DELIVERY>
	</SHOPITEM>
</SHOP>";
            }
			else if (feed.ToLower() == "olcsobbat")
            {
				testFeed = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<catalog>
	<product>
		<id>112</id>
		<manufacturer>Elenora</manufacturer>
		<name>Női Türkiz regalit karkötő - Timothea</name>
		<netprice>5790</netprice>
		<grossprice>5790</grossprice>
		<deliveryprice>0</deliveryprice>
		<urlsite>https://www.elenora.hu/karkoto/timothea-turkiz-regalit?utm_source=olcsobbat&amp;utm_medium=cpc&amp;utm_campaign=timothea-turkiz-regalit</urlsite>
		<urlpicture>https://www.elenora.hu/images/products/timothea-turkiz-regalit/timothea-turkiz-regalit-600.jpg</urlpicture>		
		<category>Ékszer és óra / Karkötő</category>
	</product>
</catalog>";
			}

			return new ContentResult
			{
				Content = testFeed,
				ContentType = "application/xml",
				StatusCode = 200
			};
		}

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
		[Route("/admin/product-feeds/{feed}")]
		public IActionResult GetFeedContentAdmin(string feed)
        {
			return Ok(productFeedService.GetFeedJson(feed));
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
		[Route("/admin/activate-product-in-feed/{feed}/{idString}")]
		public IActionResult ActivateProductInFeed(string idString, int feed)
		{
			var product = context.Products
				.Include(p => p.ProductFeedVisibilities)
				.First(p => p.IdString == idString);
			var visibility = product.ProductFeedVisibilities
				.FirstOrDefault(v => v.ProductId == product.Id && (int)v.FeedType == feed);
			if (visibility == null)
            {
				visibility = new ProductFeedVisibility
				{
					ProductId = product.Id,
					FeedType = (ProductFeedTypeEnum)feed
				};
				product.ProductFeedVisibilities.Add(visibility);
            }
			visibility.Visible = true;
			context.SaveChanges();
			return Ok();
		}
	}
}
