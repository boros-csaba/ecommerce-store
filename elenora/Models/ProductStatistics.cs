using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ProductStatistics
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public DateTime Date { get; set; }
        public int Views { get; set; }
        public int AddToCarts { get; set; }
        public int Purchases { get; set; }
    }
}
