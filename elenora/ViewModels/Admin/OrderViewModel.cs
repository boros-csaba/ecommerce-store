using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime OrderedDate { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CartContent { get; set; }
        public decimal Price { get; set; }
        public string Referrer { get; set; }
        public int PaymentRequestEmailsSent { get; set; }
        public DateTime? LastPaymentRequestEmailDate { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingName { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingCity { get; set; }
        public string BillingAddress { get; set; }
        public string BillingName { get; set; }
        public string Phone { get; set; }
        public string Remark { get; set; }
        public string AdminRemark { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public string PackageTrackingNumber { get; set; }
        public string ShippingPointAddressInformation { get; set; }
        public int OrdersCount { get; set; }
        public decimal UserTotal { get; set; }
        public OrderHistoryItem[] OrderHistories { get; set; }
        public OrderItem[] OrderItems { get; set; }
    }
}
