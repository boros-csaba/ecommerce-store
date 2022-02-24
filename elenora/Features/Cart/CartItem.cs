using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public abstract class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
        public abstract string Name { get; }
        public abstract decimal ItemPrice { get; }
        public abstract decimal? ItemOriginalPrice { get; }
        public List<CartItemComplementaryProduct> CartItemComplementaryProducts { get; set; }
    }
}
