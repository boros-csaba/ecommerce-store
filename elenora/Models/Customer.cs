using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string CookieId { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int? CartId { get; set; }
        public Cart Cart { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Referrer { get; set; }
        public string Email { get; set; }
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }
        public CustomerPopupStats CustomerPopupStats { get; set; }
        public DateTime? ReviewEmailSent { get; set; }
        public List<Order> Orders { get; set; }
        public List<WishlistItem> WishlistItems { get; set; }
        public List<ActionLog> ActionLogs { get; set; }
        public List<QuizResult> QuizResults { get; set; }
        public List<BraceletPreviewRequest> BraceletPreviewRequests { get; set; }
    }
}
