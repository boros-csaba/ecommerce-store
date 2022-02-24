using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ComponentFamilyHoroscope
    {
        [Key]
        public int Id { get; set; }
        public int ComponentFamilyId { get; set; }
        public ComponentFamily ComponentFamily { get; set; }
        public int HoroscopeId { get; set; }
        public Horoscope Horoscope { get; set; }
    }
}
