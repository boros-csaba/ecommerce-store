using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string BodyHtml { get; set; }
        public string BodyText { get; set; }
        public DateTime SentDate { get; set; }
        public string Category { get; set; }
    }
}
