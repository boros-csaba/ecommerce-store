using elenora.Models;
using elenora.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.RelatedProducts
{
    public interface IRelatedProductsService
    {
        public List<ProductListItemViewModel> GetRelatedProducts(int productId, ProductTypeEnum productType);
    }
}
