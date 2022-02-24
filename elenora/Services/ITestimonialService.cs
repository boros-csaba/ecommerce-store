using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface ITestimonialService
    {
        public List<Testimonial> GetTestimonials(int skip, int count, int seed);
        public int GetTestimonialsCount();
    }
}
