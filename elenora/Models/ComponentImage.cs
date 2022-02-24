using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ComponentImage
    {
        [Key]
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public int VariationNr { get; set; }
        public int Frequency { get; set; }
    }
}
