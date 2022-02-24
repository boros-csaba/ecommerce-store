using elenora.Models;
using elenora.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IOrderService
    {
        Order GetCustomerActiveOrder(int customerId);
        Order GetOrderById(int orderId);
        Order GetCustomerOrder(string orderId, string customerId);
        void PlaceOrder(int orderId, bool newsletterConsent);
        Payment CreateNewPayment(Order order);
        Payment GetPaymentByTransactionId(string transactionId);
        void UpdatePayment(Payment payment, string externalPaymentId, string externalTransactionId, double? fraudRiskScore, string status);
        List<FieldValidationError> CompleteShippingMethodStep(int customerId, bool validate, string emailAddress, ShippingMethodEnum shippingMethod, string shippingPointAddressInformation);
        List<FieldValidationError> CompleteBillingInformationStep(int customerId, bool validate, string billingName, string billingZip, string billingCity, string billingAddress, string phone, string remark, PaymentMethodEnum paymentMethod, bool differentShippingAddress, string shippingName = null, string shippingZip = null, string shippingCity = null, string shippingAddress = null);
        void SendAbandonedCartEmailSequences();
        void SendOrderPaymentReceivedEmail(Order order);
        void SendPaymentRequestEmail(int order);
        void SendOrderCompletedEmail(Order order);
        void ChangePaymentMethod(string orderId, string customerId, PaymentMethodEnum paymentMethod);
        void LogOrderAction(int orderId, OrderHistoryActionEnum action, string description);
    }
}
