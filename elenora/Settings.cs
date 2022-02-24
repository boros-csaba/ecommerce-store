using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora
{
    public static class Settings
    {
        public static readonly bool IS_FREE_SHIPPING_MODE = true;

        public static decimal GLS_SHIPPING_PRICE(bool isFreeShippingPromotion) => IS_FREE_SHIPPING_MODE || isFreeShippingPromotion ? 0 : 1090;
        public static decimal GLS_CSOMAGPONT_SHIPPING_PRICE(bool isFreeShippingPromotion) => IS_FREE_SHIPPING_MODE || isFreeShippingPromotion ? 0 : 990;

        public static readonly decimal GLS_PAYMENT_PRICE = 890;
        public static readonly decimal FREE_SHIPPING_THRESHOLD = 9000;
        public static readonly bool FREE_SHIPPING_INCLUDES_PAYMENT = false;

        public static readonly decimal AVERAGE_BRACELET_COST = 800;
        public static readonly decimal BRACELET_BAG_COST = 142;
        public static readonly decimal BRACELET_ENVELOPE_PRICE = 27;
    }
}
