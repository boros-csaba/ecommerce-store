using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class EmailHistory
    {
        [Key]
        public int Id { get; set; }
        public int EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }
        public int EmailTemplateId { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public DateTime SentDate { get; set; }
        public bool Unsubscribed { get; set; }
    }
}
