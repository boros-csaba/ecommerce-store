using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ProductComponent
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Bracelet Product { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public int Count { get; set; }
        public bool ShowOnProduct { get; set; }
    }
}
