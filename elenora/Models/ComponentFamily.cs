using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ComponentFamily
    {
        [Key]
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string ArticlesDescription { get; set; }
        public List<Component> Components { get; set; }
        public List<ComponentFamilyHoroscope> ComponentFamilyHoroscopes { get; set; }
        public List<ComponentFamilyChakra> ComponentFamilyChakras { get; set; }
    }
}
