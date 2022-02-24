using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IFaqService
    {
        public List<Faq> GetFaqs(FaqLocationEnum location);
        public void LogFaqOpen(int faqId, FaqLocationEnum location);
        public string GetDeliveryTimeFaqAnswer();
    }
}
