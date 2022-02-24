using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ComponentFamilyChakra
    {
        [Key]
        public int Id { get; set; }
        public int ComponentFamilyId { get; set; }
        public ComponentFamily ComponentFamily { get; set; }
        public int ChakraId { get; set; }
        public Chakra Chakra { get; set; }
    }
}
