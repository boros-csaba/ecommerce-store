using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string TransactionId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string Status { get; set; }
        public int FraudRiskScore { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
