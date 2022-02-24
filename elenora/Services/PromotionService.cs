using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly DataContext context;
        private List<Promotion> cache = null;
        private DateTime cacheUpdated = DateTime.MinValue;
        private List<Promotion> Cache
        {
            get
            {
                if (cache == null || cacheUpdated.AddHours(1) < Helper.Now)
                {
                    cache = context.Promotions.ToList();
                    cacheUpdated = Helper.Now;
                }
                return cache;
            }
        }

        public PromotionService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool IsPromotionActive(PromotionEnum promotionEnum)
        {
            var promotion = Cache.Where(p => 
                p.Type == promotionEnum &&
                p.StartDate < Helper.Now && (p.EndDate == null || Helper.Now < p.EndDate)).FirstOrDefault();
            return promotion != null;
        }

        public Promotion GetCurrentOrNextPromotion(PromotionEnum promotionEnum)
        {
            return Cache.OrderBy(p => p.StartDate).FirstOrDefault(p => 
                p.Type == promotionEnum && (p.EndDate == null || Helper.Now < p.EndDate));
        }

        public List<Promotion> GetActivePromotions()
        {
            return Cache.Where(p => p.StartDate <= Helper.Now && (p.EndDate == null || Helper.Now < p.EndDate)).ToList();
        }
    }
}
