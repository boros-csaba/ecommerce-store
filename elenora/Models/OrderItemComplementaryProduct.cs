using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class OrderItemComplementaryProduct
    {
        [Key]
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public int ComplementaryProductId { get; set; }
        public ComplementaryProduct ComplementaryProduct { get; set; }
    }
}
