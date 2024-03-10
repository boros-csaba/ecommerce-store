using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace elenora.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }
        public string Phone { get; set; }
        public int BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public bool DifferentShippingAddress { get; set; }
        public string Remark { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? OrderedDate { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public int? CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public decimal Total { get; set; }
        public int EmailSequenceStatus { get; set; }
        public DateTime? LastEmailSequenceSentDate { get; set; }
        public List<Payment> Payments { get; set; }
        public OrderPopupStats OrderPopupStats { get; set; }
        public int PaymentRequestEmailsSent { get; set; }
        public DateTime? LastPaymentRequestEmailDate { get; set; }
        public string PackageTrackingNumber { get; set; }
        public string AdminRemark { get; set; }
        public List<OrderHistory> OrderHistories { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal CouponValue { get; set; }
        public string ShippingPointAddressInformation { get; set; }

        public decimal EstimatedBusinessValue
        {
            get
            {
                var total = Total;
                var braceletsCount = OrderItems.Sum(ci => ci.Quantity);

                if (PaymentMethod == PaymentMethodEnum.PayAtDelivery)
                    total -= 2080;
                else if (ShippingMethod == ShippingMethodEnum.GLS)
                    total -= 1390;

                total -= Settings.BRACELET_ENVELOPE_PRICE;
                total -= braceletsCount * Settings.BRACELET_BAG_COST;
                total -= braceletsCount * Settings.AVERAGE_BRACELET_COST;
                
                return total;
            }
        }
    }
}
