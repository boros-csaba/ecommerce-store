using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class TestimonialsViewModel
    {
        public List<Testimonial> Testimonials { get; set; }
        public int RandomSeed { get; set; }
        public int TotalCount { get; set; }
    }
}
