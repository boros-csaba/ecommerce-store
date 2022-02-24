using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public abstract class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public abstract string ProductIdString { get; }
        public abstract string Name { get; }
        public List<OrderItemComplementaryProduct> OrderItemComplementaryProducts { get; set; }
    }
}
