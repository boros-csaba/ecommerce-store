using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewComponents
{
    public class TestimonialsViewComponent : ViewComponent
    {
        private readonly ITestimonialService testimonialService;

        public TestimonialsViewComponent(ITestimonialService testimonialService)
        {
            this.testimonialService = testimonialService ?? throw new ArgumentNullException(nameof(testimonialService));
        }

        public IViewComponentResult Invoke()
        {
            var random = new Random();
            var seed = random.Next();
            var model = new TestimonialsViewModel
            {
                Testimonials = testimonialService.GetTestimonials(0, 4, seed),
                TotalCount = testimonialService.GetTestimonialsCount(),
                RandomSeed = seed
            };
            return View(model);
        }
    }
}
