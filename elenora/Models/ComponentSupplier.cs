using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ComponentSupplier
    {
        [Key]
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Supplier { get; set; }
        public decimal LastPrice { get; set; }
        public string Link { get; set; }
        public string Remark { get; set; }
    }
}
