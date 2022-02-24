using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Chakra
    {
        [Key]
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public List<ComponentFamilyChakra> ComponentFamilyChakras { get; set; }
    }
}
