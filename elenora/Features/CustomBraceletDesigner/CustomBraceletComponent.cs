using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class CustomBraceletComponent
    {
        [Key]
        public int Id { get; set; }
        public int? CartItemId { get; set; }
        public CustomBraceletCartItem CartItem { get; set; }
        public int? OrderItemId { get; set; }
        public CustomBraceletOrderItem OrderItem { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public int Position { get; set; }
    }
}
