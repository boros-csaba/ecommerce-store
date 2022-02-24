using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public OrderHistoryActionEnum Action { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
