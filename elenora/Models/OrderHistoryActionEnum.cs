using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public enum OrderHistoryActionEnum
    {
        EmailSent = 1,
        SuccessfulPaymentReceived = 2,
        FailedPaymentReceived = 3,
        PaymentMethodChanged = 4,
        StartPayment = 5
    }
}
