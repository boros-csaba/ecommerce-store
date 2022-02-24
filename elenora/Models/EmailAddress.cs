using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class EmailAddress
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime AddedDate { get; set; }
        public string Source { get; set; }
        public bool Unsubscribed { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Order> Orders { get; set; }
        public List<EmailLog> EmailLogs { get; set; }
        public List<BraceletPreviewRequest> BraceletPreviewRequests { get; set; }
        public List<EmailHistory> EmailHistories { get; set; }
    }
}
