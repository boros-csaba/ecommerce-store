using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CartItem> CartItems { get; set; }
        public int? CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public string GetCouponWarning(decimal couponValue)
        {
            if (Coupon == null) return string.Empty;
            if (Coupon.Percentage.HasValue && CartItems.Any(ci => ci.ItemOriginalPrice != null))
            {
                return "Akciós termékekre nem vontkozik a kupon!";
            }
            else if (Coupon.MinCartValue != null && couponValue == 0)
            {
                return $"Csak {Helper.GetFormattedMoney(Coupon.MinCartValue.Value)} Ft kosárérték felett használható fel!";
            }
            else if (Coupon.GetOneFreeMinimumQuantity > 0 && CartItems.Sum(ci => ci.Quantity) < 2)
            {
                return "Tegyél legalább két karkötőt a kosaradba és az egyiket ingyen adjuk!";
            }
            return string.Empty;
        }
    }
}
