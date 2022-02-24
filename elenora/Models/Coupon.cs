using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsageCount { get; set; }
        public int? ProductCategoryId { get; set; } 
        public decimal? Value { get; set; }
        public int? Percentage { get; set; }
        public decimal? MinCartValue { get; set; }
        public int? GetOneFreeMinimumQuantity { get; set; }
    }
}
