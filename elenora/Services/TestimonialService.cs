using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly DataContext context;

        public TestimonialService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Testimonial> GetTestimonials(int skip, int count, int seed)
        {
            var random = new Random(seed);
            var testimonials = context.Testimonials.Where(t => t.Active).OrderBy(t => t.Id).ToList()
                .OrderBy(t => random.Next()).Skip(skip).Take(count).ToList();
            return testimonials;
        }

        public int GetTestimonialsCount()
        {
            return context.Testimonials.Count(t => t.Active);
        }
    }
}
