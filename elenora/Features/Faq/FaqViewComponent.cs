using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace elenora.ViewComponents
{
    public class FaqViewComponent : ViewComponent
    {
        private readonly IFaqService faqService;
        

        public FaqViewComponent(IFaqService faqService)
        {
            this.faqService = faqService ?? throw new ArgumentNullException(nameof(faqService));
        }

        public IViewComponentResult Invoke(FaqLocationEnum faqLocation)
        {
            ViewBag.FaqLocationId = (int)faqLocation;
            var faqs = faqService.GetFaqs(faqLocation);
            return View(faqs);
        }
    }
}
