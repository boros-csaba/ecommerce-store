using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class OrderPopupStats
    {
        [Key]
        public int Id { get; set; }
        public int? PopupId { get; set; }
        public Popup Popup { get; set; }
        public DateTime? PopupLastDisplayed { get; set; }
        public int PopupDisplayedCount { get; set; }
        public int PopupActionExecutedCount { get; set; }
        public string PopupDisplayRemark { get; set; }
    }
}
