using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ActionLog
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string IpAddress { get; set; }
        public ActionEnum Action { get; set; }
        public int? ProductId { get; set; }
        public Bracelet Product { get; set; }
        public DateTime Date { get; set; }
        public string Remark { get; set; }
        public string Referrer { get; set; }
        public string DeviceType { get; set; }
    }
}
