using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public enum OrderStatusEnum
    {
        NewOrder = 1,
        ShippingMethodSelected = 2,
        PaymentInformationProvided = 3,
        OrderPlaced = 10,
        PaymentReceived = 20,
        OrderPrepared = 90,
        OrderCompleted = 100
    }
}
