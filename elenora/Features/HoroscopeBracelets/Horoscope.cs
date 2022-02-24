using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Horoscope
    {
        [Key]
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string DateRange { get; set; }
        public List<ComponentFamilyHoroscope> ComponentFamilyHoroscopes { get; set; }
    }
}
