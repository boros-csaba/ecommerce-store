using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PromotionEnum Type { get; set; }
        public decimal MinOrderValue { get; set; }
        public TimeSpan RemainingTime
        {
            get
            {
                if (EndDate.HasValue) return EndDate.Value - Helper.Now;
                else return TimeSpan.MaxValue;
            }
        }
    }
}
