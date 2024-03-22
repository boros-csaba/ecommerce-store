using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using elenora.Features.Analytics;

namespace elenora.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : BaseController
    {
        private readonly IProductService productService;
        private readonly IWishlistService wishlistService;
        private readonly IActionLogService actionLogService;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly ICustomerService customerService;
        private readonly IWebHostEnvironment environment;
        private readonly IArticleService articleService;
        private readonly IPdfService pdfService;
        private readonly IFaqService faqService;
        private readonly IQuizService quizService;
        private readonly ITestimonialService testimonialService;
        private readonly IProductImageService productImageService;
        private readonly IPromotionService promotionService;
        private readonly IAnalyticsService analyticsService;
        // todo
        public HomeController(/*IConfiguration configuration, ICustomerService customerService, IProductService productService, IWishlistService wishlistService, IActionLogService actionLogService, IEmailService emailService, IArticleService articleService, IPdfService pdfService, IFaqService faqService, IQuizService quizService, ITestimonialService testimonialService, IProductImageService productImageService, IPromotionService promotionService, IAnalyticsService analyticsService, IWebHostEnvironment environment*/) //: base(configuration, customerService, promotionService)
        {
            /*this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.faqService = faqService ?? throw new ArgumentNullException(nameof(faqService));
            this.quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            this.testimonialService = testimonialService ?? throw new ArgumentNullException(nameof(testimonialService));
            this.productImageService = productImageService ?? throw new ArgumentNullException(nameof(productImageService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            this.analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            this.environment = environment;*/
        }

        [HttpGet]
        public IActionResult Index()
        {
            // todo
            return Ok("test");
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewHomePage, null, null);

            var wishlist = wishlistService.GetWishlistProductIds(CustomerId);

            var featuredProducts = productService.GetFeaturedProducts(4).Select(p => new ProductListItemViewModel(p)).ToList();
            featuredProducts.ForEach(p => p.InWishlist = wishlist.Contains(p.Id));

            var category = productService.GetCategory("noi-karkotok");
            var womenBracelets = productService.GetProductsByCategory(category, 4).Select(p => new ProductListItemViewModel(p)).ToList();
            womenBracelets.ForEach(p => p.InWishlist = wishlist.Contains(p.Id));

            category = productService.GetCategory("ferfi-karkotok");
            var menBracelets = productService.GetProductsByCategory(category, 4).Select(p => new ProductListItemViewModel(p)).ToList();
            menBracelets.ForEach(p => p.InWishlist = wishlist.Contains(p.Id));

            category = productService.GetCategory("paros-karkotok");
            var pairBracelets = productService.GetProductsByCategory(category, 4).Select(p => new ProductListItemViewModel(p)).ToList();
            pairBracelets.ForEach(p => p.InWishlist = wishlist.Contains(p.Id));

            var discountedProducts = productService.GetDiscountedProducts().Select(p => new ProductListItemViewModel(p)).ToList();

            var viewModel = new HomePageViewModel
            {
                FeaturedProducts = featuredProducts,
                WomenBracelets = womenBracelets,
                MenBracelets = menBracelets,
                PairBracelets = pairBracelets,
                DiscountedBracelets = discountedProducts
            };
            return View(viewModel);
        }

        [HttpGet]
        [Route("/xxx")]
        public IActionResult xxx()
        {
            return View();
        }

        [Route("/karkoto/{idString}")]
        public IActionResult ProductDetails(string idString)
        {
            var product = productService.GetBracelet(idString);
            if (product == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }

            productService.LogProductView(product.Id);
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.ViewProduct, product.Id, null);
            ViewBag.FbEventId = analyticsService.ReportProductView(product, Request, CookieId);

            var model = new ProductViewModel(product, environment.WebRootPath)
            {
                Promotions = promotionService.GetActivePromotions()
            };
            var wishlist = wishlistService.GetWishlistProductIds(CustomerId);
            model.InWishlist = wishlist.Contains(product.Id);

            var components = productService.GetProductComponents(product.Id, true);
            model.Components = components.Select(c => new ComponentViewModel
            {
                Name = c.Name,
                Description = c.ComponentFamily?.Description,
                ImageUrl = c.ImageUrl,
                ExampleImageUrl = c.ExampleImageUrl,
                ExampleImageDescription = c.ExampleImageDescription
            }).ToList();
            return View("ProductDetails", model);
        }

        [HttpGet]
        [Route("/cookie-hasznalat")]
        public IActionResult CookiePolicy()
        {
            return View();
        }

        [HttpPost]
        [Route("/cookie-hasznalat")]
        public IActionResult AcceptCookies()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = new DateTimeOffset(Helper.Now.AddYears(10))
            };
            Response.Cookies.Append("cookieConsent", "true", cookieOptions);
            return Ok();
        }

        [Route("/szallitasi-informaciok")]
        public IActionResult ShippingInformation()
        {
            return View();
        }

        [Route("/asvany-lexikon")]
        public IActionResult MineralsList()
        {
            var components = productService.GetComponentsCategories().Select(c => new MineralsListItemViewModel
            {
                Name = c.Name,
                Description = c.Description,
                ArticlesDescription = c.ArticlesDescription,
                Images = c.Components.Where(cc => !string.IsNullOrWhiteSpace(cc.ImageUrl)).Select(c => c.ImageUrl).ToList()
            }).Where(c => c.Images.Count > 0 && !string.IsNullOrWhiteSpace(c.Description))
            .OrderBy(c => c.Name).ToList();
            return View(components);
        }

        [Route("/report-error")]
        public IActionResult ReportError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return Ok();

            if (message.Contains("bugsnag"))
            {
                return Ok();
            }
            if (configuration.GetValue<bool>("Settings:SendErrorReports"))
            {
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "JS Error", message, message, "error");
            }
            return Ok();
        }

        [Route("/popup-kupon")]
        [HttpPost]
        public IActionResult AcceptPopup(string emailAddress)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.AcceptPopup, null, emailAddress);

            customerService.AcceptPopupOffer(CustomerId, emailAddress);
            return Ok();
        }

        [Route("/popup-megjelenes")]
        [HttpPost]
        public IActionResult ShowPopup(string sourcePage)
        {
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.DisplayPopup, null, sourcePage);

            customerService.ShowPopup(CustomerId, sourcePage);
            return Ok();
        }

        [Route("/cikkek")]
        public IActionResult Articles()
        {
            var model = articleService.GetArticles().Select(a =>
                new ArticleSummaryViewModel
                {
                    IdString = a.IdString,
                    Title = a.Title,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl
                }
            ).ToList();
            return View(model);
        }

        [Route("/cikkek/{articleId}")]
        public IActionResult Article(string articleId)
        {
            var article = articleService.GetArticle(articleId);
            if (article == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            var model = new ArticleViewModel(article);
            if (article.Content.Contains(@"<div class=""quiz""></div>"))
            {
                model.Beads = productService.GetBraceletDesignerComponents()
                    .Select(c => new ComponentViewModel
                    {
                        ImageUrl = c.ImageUrl.Split(".").First().Split("/").Last(),
                        Name = c.Name,
                        Description = c.ComponentFamily.Description
                    }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        [Route("/quiz/start")]
        public IActionResult StartQuiz(string quizName)
        {
            var quiz = quizService.StartNewQuiz(CustomerId, quizName);
            return Ok(quiz.Id);
        }

        [HttpPost]
        [Route("/quiz/save")]
        public IActionResult SaveQuizStep(int quizId, int answerCount, string answer)
        {
            quizService.SaveQuizAnswer(CustomerId, quizId, answerCount, answer);
            return Ok();
        }

        [HttpPost]
        [Route("/quiz/save-result")]
        public IActionResult SaveQuizResult(int quizId, string result)
        {
            quizService.SaveQuizResult(CustomerId, quizId, result);
            return Ok();
        }

        [Route("/aszf")]
        public IActionResult Legal()
        {
            FillViewBagPromotions();
            return View();
        }

        [Route("/aszf-pdf")]
        public IActionResult LegalPdf()
        {
            FillViewBagPromotions();
            return GetPdfFromView("_Legal", "ÁSZF.pdf");
        }

        private void FillViewBagPromotions()
        {
            var freeShippingPromotion = promotionService.GetCurrentOrNextPromotion(PromotionEnum.FreeShipping);
            if (freeShippingPromotion != null)
            {
                ViewBag.FreeShippingStartDate = freeShippingPromotion.StartDate;
                ViewBag.FreeShippingEndDate = freeShippingPromotion.EndDate;
            }
            var giftLavaBraceletPromotion = promotionService.GetCurrentOrNextPromotion(PromotionEnum.GiftLavaBracelet);
            if (giftLavaBraceletPromotion != null)
            {
                ViewBag.GiftLavaBraceletStartDate = giftLavaBraceletPromotion.StartDate;
                ViewBag.GiftLavaBraceletEndDate = giftLavaBraceletPromotion.EndDate;
            }
        }

        [Route("/aszf-elallas-pdf")]
        public IActionResult LegalWithdrawalPdf()
        {
            return GetPdfFromView("_LegalWithdrawal", "Elállási nyilatkozatminta.pdf");
        }

        [Route("/adatkezelesi-tajekoztato")]
        public IActionResult DataPolicy()
        {
            return View();
        }

        [Route("/adatkezelesi-tajekoztato-pdf")]
        public IActionResult DataPolicyPdf()
        {
            return GetPdfFromView("_DataPolicy", "Adatkezelési tájékoztató.pdf");
        }

        [Route("/impresszum")]
        public IActionResult Impressum()
        {
            return View();
        }

        private FileContentResult GetPdfFromView(string viewName, string fileName)
        {
            using (StringWriter writer = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer, new HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext).Wait();
                var html = writer.GetStringBuilder().ToString();
                var pdfBytes = pdfService.GeneratePdf(html);
                return File(pdfBytes, "application/pdf", fileName);
            }
        }

        [Route("/gyakori-kerdesek")]
        public IActionResult FAQ()
        {
            return View();
        }

        [HttpPost]
        [Route("/gyakori-kerdesek/{id}-{locationId}")]
        public IActionResult FAQOpen(int id, int locationId)
        {
            faqService.LogFaqOpen(id, (FaqLocationEnum)locationId);
            return Ok();
        }

        [Route("/rolunk")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        [Route("/velemenyek/{skip}-{seed}")]
        public IActionResult Testimonials(int skip, int seed)
        {
            var testimonials = testimonialService.GetTestimonials(skip, 3, seed);
            return Ok(testimonials);
        }

        [Route("/product-images/{cartItemId}-{customerId}.jpg")]
        public IActionResult Image(int cartItemId, int customerId, int? s)
        {
            try
            {
                var image = productImageService.GetCustomBraceletImage(cartItemId, customerId, environment.WebRootPath, s ?? 500);
                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                var errorText = $"{cartItemId}-{customerId} {e}";
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Elenora kép HIBA", errorText, errorText, "error");
                var imagePath = Path.Combine(environment.WebRootPath, "/images/custom-bracelet-placeholder.png")
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", @"{Path.DirectorySeparatorChar}");
                return File(imagePath, "image/jpeg");
            }
        }

        [Route("/ordered-product-images/{orderItemId}-{customerId}.jpg")]
        public IActionResult OrderItemImage(int orderItemId, int customerId, int? s)
        {
            try
            {
                var image = productImageService.GetCustomOrderedBraceletImage(orderItemId, customerId, environment.WebRootPath, s ?? 500);
                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                var errorText = $"{orderItemId}-{customerId} {e}";
                emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Elenora kép HIBA", errorText, errorText, "error");
                var imagePath = Path.Combine(environment.WebRootPath, "/images/custom-bracelet-placeholder.png")
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", @"{Path.DirectorySeparatorChar}");
                return File(imagePath, "image/jpeg");
            }
        }

        [Route("/leiratkozas")]
        public IActionResult Unsubscribe(string p)
        {
            emailService.Unsubscribe(p);
            return View();
        }

        [Route("/hiba")]
        public IActionResult Error()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var errorMessage = $"{error.Path} - {error.Error}";
            actionLogService.LogAction(CustomerId, UserHelper.GetActionLogInformation(Request), ActionEnum.Error, null, errorMessage);
            emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Elenora HIBA", errorMessage, errorMessage, "error");
            return View("Error");
        }

        [Route("/robots.txt")]
        public IActionResult RobotsTxt()
        {
            return NotFound();
        }

        [Route("/wp-login.php")]
        [Route("/apple-app-site-association")]
        [Route("/humans.txt")]
        [Route("/ads.txt")]
        [Route("/install/")]
        [Route("/wp/")]
        [Route("/test/")]
        [Route("/site/")]
        [Route("/umbraco/")]
        [Route("/xmlrpc.php")]
        [HttpGet]
        public IActionResult NotFoundPages()
        {
            return NotFound();
        }
    }
}
