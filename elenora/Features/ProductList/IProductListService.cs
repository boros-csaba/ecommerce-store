using elenora.Features.ProductList.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductList
{
    public interface IProductListService
    {
        public ProductList GetProductList(int customerId, ProductCategoryTypeEnum categoryType, string category, int page);
        public ProductList GetProductList(int customerId, string mineral, int page);
        public ProductCategoryModel GetProductCategory(int customerId, ProductCategoryTypeEnum categoryType);
        public ProductCategoryModel GetProductsByMinerals(int customerId);
        public List<Admin.ProductCategoryNode> GetProductsAdminReport();
    }
}
