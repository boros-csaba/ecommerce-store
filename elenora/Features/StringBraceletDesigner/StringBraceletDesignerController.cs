using elenora.Controllers;
using elenora.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.StringBraceletDesigner
{
    public class StringBraceletDesignerController : BaseController
    {
        private readonly ICartService cartService;
        private readonly DataContext context;

        public StringBraceletDesignerController(DataContext context, ICartService cartService, IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("/fonott-karkoto-keszito")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/fonott-karkoto-keszito/kosarba")]
        public IActionResult AddToCart(BraceletTypeEnum braceletType, string knotColor, string string1Color, string string2Color, string string3Color, string flap1Color, string flap2Color)
        {
            cartService.AddToCart(CustomerId, braceletType, knotColor, string1Color, string2Color, string3Color, flap1Color, flap2Color);
            return Ok();
        }

        [Route("/ordered-product-images-string/{orderItemId}-{customerId}.svg")]
        public IActionResult OrderItemImage(int orderItemId, int customerId)
        {
            var orderItem = context.OrderItems.First(o => o.Id == orderItemId && o.Order.CustomerId == customerId);
            var model = new StringBraceletViewModel(orderItem as StringBraceletOrderItem);
            model.Width = 600;
            model.Height = 500;
            return View("_StringBraceletImage", model);
        }
    }
}
