using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IPromotionService
    {
        public bool IsPromotionActive(PromotionEnum promotion);
        public Promotion GetCurrentOrNextPromotion(PromotionEnum promotion);
        public List<Promotion> GetActivePromotions();
    }
}
