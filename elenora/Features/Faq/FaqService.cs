using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class FaqService : IFaqService
    {
        private readonly DataContext context;
        private readonly IPromotionService promotionService;
        private const int deliveryTimeFaqId = 1;

        public FaqService(DataContext context, IPromotionService promotionService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        public List<Faq> GetFaqs(FaqLocationEnum location)
        {
            IEnumerable<Faq> faqs = new List<Faq>();
            switch (location)
            {
                case FaqLocationEnum.FaqPage:
                    faqs = context.Faqs.Where(f => f.FaqPageOrder > 0).OrderBy(f => f.FaqPageOrder);
                    break;
                case FaqLocationEnum.BraceletDesigner:
                    faqs = context.Faqs.Where(f => f.BraceletDesignerOrder > 0).OrderBy(f => f.BraceletDesignerOrder);
                    break;
                case FaqLocationEnum.ProductDetails:
                    faqs = context.Faqs.Where(f => f.ProductDetailsOrder > 0).OrderBy(f => f.ProductDetailsOrder);
                    break;
                case FaqLocationEnum.CartPage:
                    faqs = context.Faqs.Where(f => f.CartPageOrder > 0).OrderBy(f => f.CartPageOrder);
                    break;
                case FaqLocationEnum.HoroscopeBracelets:
                    faqs = context.Faqs.Where(f => f.HoroscopeBraceletsOrder > 0).OrderBy(f => f.HoroscopeBraceletsOrder);
                    break;
            };
            foreach (var faq in faqs)
            {
                faq.FormattedAnswer = GetFormattedAnswer(faq.Answer);
            }
            return faqs.ToList();
        }

        public void LogFaqOpen(int faqId, FaqLocationEnum location)
        {
            var faq = context.Faqs.First(f => f.Id == faqId);
            switch (location)
            {
                case FaqLocationEnum.FaqPage:
                    faq.FaqPageOpenCount++;
                    break;
                case FaqLocationEnum.BraceletDesigner:
                    faq.BraceletDesignerOpenCount++;
                    break;
                case FaqLocationEnum.ProductDetails:
                    faq.ProductDetailsOpenCount++;
                    break;
                case FaqLocationEnum.CartPage:
                    faq.CartPageOpenCount++;
                    break;
                case FaqLocationEnum.HoroscopeBracelets:
                    faq.HoroscopeBraceletsOpenCount++;
                    break;
            }
            context.SaveChanges();
        }

        public string GetDeliveryTimeFaqAnswer()
        {
            var faq = context.Faqs.First(f => f.Id == deliveryTimeFaqId);
            return GetFormattedAnswer(faq.Answer);
        }

        private string GetFormattedAnswer(string asnwer)
        {
            string result = asnwer;
            if (result.Contains("[EarliestDeliveryDay]"))
            {
                result = result.Replace("[EarliestDeliveryDay]", GetEarliestDeliveryDay());
            }
            if (result.Contains("[LatestDeliveryDay]"))
            {
                result = result.Replace("[LatestDeliveryDay]", GetLatestDeliveryDay());
            }
            if (result.Contains("[GLSDeliveryPrice]"))
            {
                result = result.Replace("[GLSDeliveryPrice]", GetGLSDeliveryPrice());
            }
            if (result.Contains("[GLSPaymentPrice]"))
            {
                result = result.Replace("[GLSPaymentPrice]", GetGLSPaymentPrice());
            }
            if (result.Contains("[FreeShippingThreshold]"))
            {
                result = result.Replace("[FreeShippingThreshold]", GetFreeShippingThreshold());
            }
            if (result.Contains("[PostaShippingPrice]"))
            {
                result = result.Replace("[PostaShippingPrice]", GetPostaShippingPrice());
            }        

            return result;
        }

        private string GetEarliestDeliveryDay()
        {
            var now = Helper.Now;
            var deliveryDate = now;
            bool isWeekend = false;
            while (deliveryDate.DayOfWeek == DayOfWeek.Saturday || deliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                deliveryDate = deliveryDate.AddDays(1);
                isWeekend = true;
            }
            if (Helper.Now.Hour < 13 || isWeekend)
            {
                deliveryDate = deliveryDate.AddDays(1);
            }
            else
            {
                deliveryDate = deliveryDate.AddDays(2);
            }
            while (deliveryDate.DayOfWeek == DayOfWeek.Saturday || deliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                deliveryDate = deliveryDate.AddDays(1);
            }
            var shippingDuration = (deliveryDate - now).Days;
            if (shippingDuration <= 1)
            {
                return "holnap";
            }
            if (shippingDuration <= 2)
            {
                return "holnapután";
            }
            return days[(int)deliveryDate.DayOfWeek - 1];
        }

        private string GetLatestDeliveryDay()
        {
            var now = Helper.Now;
            var deliveryDate = now;
            bool isWeekend = false;
            while (deliveryDate.DayOfWeek == DayOfWeek.Saturday || deliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                deliveryDate = deliveryDate.AddDays(1);
                isWeekend = true;
            }
            if (Helper.Now.Hour < 13 || isWeekend)
            {
                deliveryDate = deliveryDate.AddDays(1);
            }
            else
            {
                deliveryDate = deliveryDate.AddDays(2);
            }
            deliveryDate = deliveryDate.AddDays(3);
            while (deliveryDate.DayOfWeek == DayOfWeek.Saturday || deliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                deliveryDate = deliveryDate.AddDays(1);
            }
            return days[(int)deliveryDate.DayOfWeek - 1];
        }

        private string GetGLSDeliveryPrice()
        {
            return Helper.GetFormattedMoney(Settings.GLS_SHIPPING_PRICE(promotionService.IsPromotionActive(PromotionEnum.FreeShipping))) + " Ft";
        }

        private string GetGLSPaymentPrice()
        {
            return Helper.GetFormattedMoney(Settings.GLS_PAYMENT_PRICE) + " Ft";
        }

        private string GetFreeShippingThreshold()
        {
            return Helper.GetFormattedMoney(Settings.FREE_SHIPPING_THRESHOLD) + " Ft";
        }

        private string GetPostaShippingPrice()
        {
            return Helper.GetFormattedMoney(Settings.GLS_CSOMAGPONT_SHIPPING_PRICE(promotionService.IsPromotionActive(PromotionEnum.FreeShipping))) + " Ft";
        }

        private readonly string[] days = new string[]
        {
            "hétfőn", "kedden", "szerdán", "csütörtökön", "pénteken"
        };
    }
}
