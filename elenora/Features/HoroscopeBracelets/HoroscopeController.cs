using elenora.Features.HoroscopeBracelets;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class HoroscopeController : BaseController
    {
        private readonly IProductService productService;
        private readonly ICartService cartService;
        private readonly IActionLogService actionLogService;
        private readonly DataContext context;

        public HoroscopeController(IConfiguration configuration, ICustomerService customerService, ICartService cartService, IProductService productService, IActionLogService actionLogService, DataContext context, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.actionLogService = actionLogService ?? throw new ArgumentNullException(nameof(actionLogService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("/horoszkop-karkotok")]
        public IActionResult Index()
        {
            var horoscopes = context.Horoscopes.OrderBy(h => h.Id).ToList();
            var model = new HoroscopePageViewModel
            {
                Horoscopes = horoscopes.Select(h => new HoroscopeViewModel(h)).ToList(),
                Sections = horoscopes.Select(h => new HoroscopeListSectionViewModel
                {
                    HoroscopeName = h.Name,
                    HoroscopeIdString = h.IdString,
                    Products = productService.GetActiveProducts(b => b.CategoryId == h.Id + 10 && b.State == ProductStateEnum.Active)
                                    .Take(4)
                                    .Select(b => new ProductListItemViewModel(b)).ToList()
                }).ToList()
            };
            return View(model);
        }

        [Route("/horoszkop-karkotok/{horoscope}-csillagjegy")]
        public IActionResult HoroscopePage(string horoscope)
        {
            var horoscopes = context.Horoscopes.OrderBy(h => h.Id).ToList();
            var selectedHoroscope = horoscopes.First(h => h.IdString == horoscope);
            var model = new HoroscopePageViewModel
            {
                Horoscopes = horoscopes.Select(h => new HoroscopeViewModel(h)).ToList(),
                Sections = horoscopes.Where(h => h.Id == selectedHoroscope.Id)
                    .Select(h => new HoroscopeListSectionViewModel
                {
                    HoroscopeName = h.Name,
                    HoroscopeIdString = h.IdString,
                    Products = productService.GetActiveProducts(b => b.CategoryId == h.Id + 10 && b.State == ProductStateEnum.Active)
                                    .Select(b => new ProductListItemViewModel(b)).ToList()
                }).ToList()


                /*
                HoroscopeBracelets = horoscopes.Select(h =>
                    new HoroscopeViewModel
                    {
                        Id = h.Id,
                        IdString = h.IdString,
                        Name = h.Name,
                        DateRange = h.DateRange,
                        Beads = h.HoroscopeBracelets
                            .Where(b => b.Active)
                            .GroupBy(g => g.BeadId)
                            .Select(b => new HoroscopeBeadViewModel
                            {
                                Id = b.First().BeadId,
                                Name = b.First().Bead.Name,
                                Description = b.First().Bead.ComponentFamily.ShortDescription,
                                ImageUrl = b.First().Bead.ImageUrl,
                                Bracelets = h.HoroscopeBracelets.Where(bb => bb.BeadId == b.First().BeadId).Select(bb =>
                                    new HoroscopeBraceletViewModel
                                    {
                                        Id = bb.Id,
                                        Name = bb.Name,
                                        Price = bb.Price.Price,
                                        IdString = bb.IdString
                                    }).ToList()
                            }).ToList()
                    }).ToList(),
                Sections = horoscopes.Select(h => new HoroscopeListSectionViewModel
                {
                    HoroscopeName = h.Name,
                    HoroscopeIdString = h.IdString,
                    Products = h.HoroscopeBracelets.Where(b => b.Active).Select(b => new ProductListItemViewModel(b)).ToList()
                }).ToList()*/
            };

            return View(model);
        }
    }
}
