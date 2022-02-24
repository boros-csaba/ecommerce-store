using System;
using System.Collections.Generic;
using System.Linq;
using elenora.Features.Inventory;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace elenora.Controllers
{
    public class CustomBraceletDesignerController : BaseController
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;
        private readonly IActionLogService actionLogService;
        private readonly IEmailService emailService;
        private readonly IFaqService faqService;
        private readonly DataContext context;
        private readonly IInventoryService inventoryService;

        public CustomBraceletDesignerController(IInventoryService inventoryService, IConfiguration configuration, ICustomerService customerService, ICartService cartService, IProductService productService, IActionLogService actionLogService, IEmailService emailService, IFaqService faqService, DataContext context, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.faqService = faqService ?? throw new ArgumentNullException(nameof(faqService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        [Route("/egyedi-karkoto-keszito2")]
        public IActionResult CustomBraceletDesignerV2()
        {
            var model = new BraceletDesignerViewModel
            {
                ExampleImages = new List<Tuple<string, string>> {
                    new Tuple<string, string>("lepidolit-karkoto.webp", "Lepidolit"),
                    new Tuple<string, string>("matt-barack-aventurin-karkoto-2.webp", "Matt barack aventurin"),
                    new Tuple<string, string>("zold-jade-karkoto-3.webp", "Zöld jáde"),
                    new Tuple<string, string>("matt-howlit-karkoto.webp", "Matt howlit"),
                    new Tuple<string, string>("matt-onix-karkoto.webp", "Matt ónix"),
                    new Tuple<string, string>("matt-szurke-kepjaspis-karkoto.webp", "Matt szűrke képjáspis"),
                    new Tuple<string, string>("rozsakvarc-karkoto.webp", "Rózsakvarc"),
                    new Tuple<string, string>("tigrisszem-karkoto.webp", "Tigrisszem"),
                    new Tuple<string, string>("zold-jade-karkoto-2.webp", "Zöld jáde"),
                    new Tuple<string, string>("hematit-karkoto.webp", "Hematit"),
                    new Tuple<string, string>("kek-jade-karkoto.webp", "Kék jáde"),
                    new Tuple<string, string>("krem-jade-karkoto.webp", "Krém jáde"),
                    new Tuple<string, string>("matt-feher-achat-karkoto.webp", "Matt fehér achát"),
                    new Tuple<string, string>("krem-jade-karkoto-2.webp", "Krém jáde"),
                    new Tuple<string, string>("lavako-karkoto.webp", "Lávakő"),
                    new Tuple<string, string>("voros-jaspis-karkoto.webp", "Vörös jáspis"),
                    new Tuple<string, string>("kek-jade-karkoto-2.webp", "Kék jáde"),
                    new Tuple<string, string>("matt-howlit-karkoto-2.webp", "Matt howlit"),
                    new Tuple<string, string>("zold-jade-karkoto.webp", "Zöld jáde"),
                    new Tuple<string, string>("matt-barack-aventurin-karkoto.webp", "Matt barack aventurin"),
                    new Tuple<string, string>("kek-jade-karkoto-3.webp", "Kék jáde")
                },
                Beads = productService.GetBraceletDesignerComponents()
                    .Select(c => new BraceletDesignerBeadViewModel
                    {
                        Id = c.Id,
                        IdString = c.IdString,
                        ImageUrl = c.ImageUrl.Split(".").First().Split("/").Last(),
                        Name = c.Name,
                        Description = c.ComponentFamily.ShortDescription,
                        LongDescription = c.ComponentFamily.Description,
                        SoldOut = inventoryService.IsBeadSoldOutInBraceletDesigner(c),
                        ComplementaryProducts = c.BeadComplementaryProducts.Select(m => new ComplementaryProductViewModel
                        {
                            Id = m.ComplementaryProductId,
                            Name = m.ComplementaryProduct.Name,
                            Price = m.ComplementaryProduct.Price,
                            ImageUrl = m.ComplementaryProduct.ImageUrl
                        }).ToList(),
                        ImageFrequencies = c.ComponentImages.OrderBy(c => c.VariationNr).Select(c => c.Frequency).ToArray()
                    }).ToList(),
                WhiteLetters = productService.GetLetters(true).Select(l => new Tuple<int, string>(l.Id, l.Name.Last().ToString())).ToList(),
                BlackLetters = productService.GetLetters(false).Select(l => new Tuple<int, string>(l.Id, l.Name.Last().ToString())).ToList(),
                DeliveryTimeFaqAnswer = faqService.GetDeliveryTimeFaqAnswer()
            };
            return View(model);
        }

        [Route("/egyedi-karkoto-keszito")]
        public IActionResult Index(int? b)
        {
            if (b != null && b > 0)
            {
                actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.StartEditCustomBracelet, null, null);
            }
            var model = new BraceletDesignerViewModel
            {
                ExampleImages = new List<Tuple<string, string>> {
                    new Tuple<string, string>("lepidolit-karkoto.webp", "Lepidolit"),
                    new Tuple<string, string>("matt-barack-aventurin-karkoto-2.webp", "Matt barack aventurin"),
                    new Tuple<string, string>("zold-jade-karkoto-3.webp", "Zöld jáde"),
                    new Tuple<string, string>("matt-howlit-karkoto.webp", "Matt howlit"),
                    new Tuple<string, string>("matt-onix-karkoto.webp", "Matt ónix"),
                    new Tuple<string, string>("matt-szurke-kepjaspis-karkoto.webp", "Matt szűrke képjáspis"),
                    new Tuple<string, string>("rozsakvarc-karkoto.webp", "Rózsakvarc"),
                    new Tuple<string, string>("tigrisszem-karkoto.webp", "Tigrisszem"),
                    new Tuple<string, string>("zold-jade-karkoto-2.webp", "Zöld jáde"),
                    new Tuple<string, string>("hematit-karkoto.webp", "Hematit"),
                    new Tuple<string, string>("kek-jade-karkoto.webp", "Kék jáde"),
                    new Tuple<string, string>("krem-jade-karkoto.webp", "Krém jáde"),
                    new Tuple<string, string>("matt-feher-achat-karkoto.webp", "Matt fehér achát"),
                    new Tuple<string, string>("krem-jade-karkoto-2.webp", "Krém jáde"),
                    new Tuple<string, string>("lavako-karkoto.webp", "Lávakő"),
                    new Tuple<string, string>("voros-jaspis-karkoto.webp", "Vörös jáspis"),
                    new Tuple<string, string>("kek-jade-karkoto-2.webp", "Kék jáde"),
                    new Tuple<string, string>("matt-howlit-karkoto-2.webp", "Matt howlit"),
                    new Tuple<string, string>("zold-jade-karkoto.webp", "Zöld jáde"),
                    new Tuple<string, string>("matt-barack-aventurin-karkoto.webp", "Matt barack aventurin"),
                    new Tuple<string, string>("kek-jade-karkoto-3.webp", "Kék jáde")
                },
                Beads = productService.GetBraceletDesignerComponents()
                    .Select(c => new BraceletDesignerBeadViewModel
                    {
                        Id = c.Id,
                        IdString = c.IdString,
                        ImageUrl = c.ImageUrl.Split(".").First().Split("/").Last(),
                        Name = c.Name,
                        Description = c.ComponentFamily.ShortDescription,
                        LongDescription = c.ComponentFamily.Description,
                        SoldOut = inventoryService.IsBeadSoldOutInBraceletDesigner(c),
                        ComplementaryProducts = c.BeadComplementaryProducts.Select(m => new ComplementaryProductViewModel
                        {
                            Id = m.ComplementaryProductId,
                            Name = m.ComplementaryProduct.Name,
                            Price = m.ComplementaryProduct.Price,
                            ImageUrl = m.ComplementaryProduct.ImageUrl
                        }).ToList(),
                        ImageFrequencies = c.ComponentImages.OrderBy(c => c.VariationNr).Select(c => c.Frequency).ToArray()
                    }).ToList(),
                WhiteLetters = productService.GetLetters(true).Select(l => new Tuple<int, string>(l.Id, l.Name.Last().ToString())).ToList(),
                BlackLetters = productService.GetLetters(false).Select(l => new Tuple<int, string>(l.Id, l.Name.Last().ToString())).ToList(),
                DeliveryTimeFaqAnswer = faqService.GetDeliveryTimeFaqAnswer()
            };
            if (b != null)
            {
                var cartItem = context.CustomBraceletCartItems
                    .Include(c => c.Components)
                    .Include(c => c.CartItemComplementaryProducts)
                    .FirstOrDefault(c => c.Id == b && c.Cart.CustomerId == CustomerId);
                if (cartItem != null)
                {
                    model.CartItemId = b;
                    model.SelectedBeadId = cartItem.BeadTypeId;
                    model.SelectedSecondaryBeadId = cartItem.SecondaryBeadTypeId ?? 0;
                    model.StyleType = cartItem.StyleType;
                    model.SelectedComponentIds = cartItem.Components.OrderBy(c => c.Position).Select(c => c.ComponentId).ToList();
                    model.SelectedComplementaryProductIds = cartItem.CartItemComplementaryProducts.Select(c => c.ComplementaryProductId).ToList();
                    model.BraceletSize = cartItem.BraceletSize.Value;
                }
            }

            ////todo
            //model.Beads[0].IsPremium = true;
            //model.Beads[1].IsPremium = true;

            return View(model);
        }

        [HttpPost]
        [Route("/egyedi-karkoto-keszito/start")]
        public IActionResult StartBraceletDesigner()
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewCustomBracelet, null, null);
            return Ok();
        }

        [Route("/egyedi-asvany-karkoto")]
        public IActionResult CustomMineralBracelet()
        {
            return View();
        }

        [HttpPost]
        [Route("/egyedi-karkoto-keszito/kosarba")]
        public IActionResult AddToCart(int cartItemId, int beadTypeId, int secondaryBeadTypeId, CustomBraceletStyleEnum styleType, int[] componentIds, BraceletSizeEnum braceletSize, int[] complementaryProducts)
        {
            if (cartItemId > 0)
            {
                actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.SaveEditedCustomBracelet, null, null);
            }
            else
            {
                actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.AddToCartCustomBracelet, null, null);
            }
            cartService.AddToCart(CustomerId, cartItemId, beadTypeId, secondaryBeadTypeId, styleType, componentIds, braceletSize, complementaryProducts);
            return Ok();
        }
    }
}