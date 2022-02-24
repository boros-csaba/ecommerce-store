using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class CartItemComplementaryProduct
    {
        [Key]
        public int Id { get; set; }
        public int CartItemId { get; set; }
        public CartItem CartItem { get; set; }
        public int ComplementaryProductId { get; set; }
        public ComplementaryProduct ComplementaryProduct { get; set; }
    }
}
