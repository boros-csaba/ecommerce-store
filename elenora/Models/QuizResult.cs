using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string QuizName { get; set; }
        public DateTime StartDate { get; set; }
        public string Answer1 { get; set; }
        public DateTime? Answer1Date { get; set; }
        public string Answer2 { get; set; }
        public DateTime? Answer2Date { get; set; }
        public string Answer3 { get; set; }
        public DateTime? Answer3Date { get; set; }
        public string Answer4 { get; set; }
        public DateTime? Answer4Date { get; set; }
        public string Answer5 { get; set; }
        public DateTime? Answer5Date { get; set; }
        public string Answer6 { get; set; }
        public DateTime? Answer6Date { get; set; }
        public string Result { get; set; }
    }
}
