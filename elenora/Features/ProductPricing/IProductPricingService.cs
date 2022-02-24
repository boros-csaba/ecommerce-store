using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    public interface IProductPricingService
    {
        public ProductPrice GetProductPrice(Bracelet product);
        public MarginsInformation GetMarginsInformation(Bracelet bracelet);
        public List<int> GetDiscountedProductIds();
        public List<OrderCostNode> GetOrderCostInformation(int orderId);
    }
}
