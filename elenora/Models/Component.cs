using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Component
    {
        [Key]
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string ExampleImageUrl { get; set; }
        public string ExampleImageDescription { get; set; }
        public int? ComponentFamilyId { get; set; }
        public ComponentFamily ComponentFamily { get; set; }
        public int Quantity { get; set; }
        public int MinimumQuantity { get; set; }
        public bool SelectabeInBraceletDesigner { get; set; }
        public List<ProductComponent> ProductComponents { get; set; }
        public List<BeadComplementaryProduct> BeadComplementaryProducts { get; set; }
        public float WidthRatio { get; set; }
        public float HeightRatio { get; set; }
        public bool Searchable { get; set; }
        public List<ComponentSupplier> ComponentSuppliers { get; set; }
        public List<ComponentImage> ComponentImages { get; set; }
        public int ImagesQuality { get; set; }
        public string Remark { get; set; }
    }
}
