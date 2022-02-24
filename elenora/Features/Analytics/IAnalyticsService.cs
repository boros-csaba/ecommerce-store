using elenora.BusinessModels;
using elenora.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.Analytics
{
    public interface IAnalyticsService
    {
        public string ReportProductView(Bracelet product, HttpRequest httpRequest, string cookieId);
        public void ReportAddToCart(int productId, int quantity, HttpRequest httpRequest, string cookieId, string eventId);
    }
}
