using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string TextContent { get; set; }
        public string HtmlContent { get; set; }
        public bool Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OnlyAfterEmailId { get; set; }
        public EmailTemplate OnlyAfterEmail { get; set; }
        public int OnlyAfterMinutes { get; set; }
    }
}
