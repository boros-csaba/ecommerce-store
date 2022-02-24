using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class BraceletPreviewRequest
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }
        public string BraceletImageUrl { get; set; }
    }
}
