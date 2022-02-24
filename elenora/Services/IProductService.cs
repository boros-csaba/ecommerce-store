using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IProductService
    {
        List<Bracelet> GetActiveProducts(Expression<Func<Bracelet, bool>> filter);
        List<Bracelet> GetFeaturedProducts(int? limit = null);
        List<Bracelet> GetDiscountedProducts();
        Bracelet GetBracelet(string idString);
        Bracelet GetBracelet(int id);
        List<Bracelet> GetProducts(List<string> productIds);
        Category GetCategory(string categoryId);
        List<Bracelet> GetProductsByCategory(Category category, int? limit = null);
        List<Bracelet> Search(string searchTerm);
        List<Component> GetProductComponents(int productId, bool onlyVisible);
        List<Component> GetComponents(List<string> componentNames);
        List<ComponentFamily> GetComponentsCategories();
        List<Component> GetBraceletDesignerComponents();
        List<Component> GetLetters(bool white);
        void LogProductView(int productId);
        void LogProductAddToCart(int productId);
        void LogProductPurchase(int productId);
    }
}
