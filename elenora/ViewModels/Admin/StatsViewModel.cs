using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class StatsViewModel
    {
        public int BankTransferOrdersAll { get; set; }
        public int BankTransferOrdersPayed { get; set; }
        public int BankTransferOrdersAllPrev { get; set; }
        public int BankTransferOrdersPayedPrev { get; set; }
        public int OrdersNumber { get; set; }
        public int OrdersNumberPrev { get; set; }
        public int OrdersInStep1 { get; set; }
        public int OrdersInStep2 { get; set; }
        public int OrdersInStep3 { get; set; }
        public int OrdersInStep4 { get; set; }
        public int OrdersInStep1Prev { get; set; }
        public int OrdersInStep2Prev { get; set; }
        public int OrdersInStep3Prev { get; set; }
        public int OrdersInStep4Prev { get; set; }
        public decimal OrdersTotal { get; set; }
        public decimal OrdersTotalPrev { get; set; }
        public int AbandonedCardPayments { get; set; }
        public int AbandonedCardPaymentsPrev { get; set; }
        public int PayedCardOrders { get; set; }
        public int PayedCardOrdersPrev { get; set; }
        public int CustomBraceletViews { get; set; }
        public int CustomBraceletAddToCart { get; set; }
        public int CustomBraceletViewsPrev { get; set; }
        public int CustomBraceletAddToCartPrev { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
        public decimal CustomerLifetimeValueNet { get; set; }
        public decimal RepeatOrdersPercentage { get; set; }
        public decimal AverageDaysBetweenOrders { get; set; }
    }
}
