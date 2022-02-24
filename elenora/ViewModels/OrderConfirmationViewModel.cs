using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public ShippingMethodEnum ShippingMethod { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public decimal Total { get; set; }
        public decimal ShippingPrice { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public int CustomerId { get; set; }
        public string CustomerIdString { get; set; }
        public string PackageTrackingNumber { get;set;}

        public OrderConfirmationViewModel(Order order)
        {
            Id = order.Id;
            OrderId = order.OrderId;
            PaymentMethod = order.PaymentMethod;
            ShippingMethod = order.ShippingMethod;
            OrderStatus = order.Status;
            Total = order.Total;
            ShippingPrice = order.ShippingPrice;
            CustomerId = order.CustomerId;
            PackageTrackingNumber = order.PackageTrackingNumber;
        }
    }
}
