using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Faq
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Answer { get; set; }
        public int FaqPageOrder { get; set; }
        public int FaqPageOpenCount { get; set; }
        public int BraceletDesignerOrder { get; set; }
        public int BraceletDesignerOpenCount { get; set; }
        public int ProductDetailsOrder { get; set; }
        public int ProductDetailsOpenCount { get; set; }
        public int CartPageOrder { get; set; }
        public int CartPageOpenCount { get; set; }
        public int HoroscopeBraceletsOrder { get; set; }
        public int HoroscopeBraceletsOpenCount { get; set; }
        [NotMapped]
        public string FormattedAnswer { get; set; }
    }
}
