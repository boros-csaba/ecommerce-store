using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class EmailTemplateViewModel
    {
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public int SentEmails { get; set; }
        public DateTime? LastSent { get; set; }
        public string Content { get; set; }
        public int UnsubscribedEmails { get; set; }
    }
}
