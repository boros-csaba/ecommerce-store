using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IWishlistService
    {
        void AddToWishlist(int productId, int customerId);
        void DeleteFromWishlist(int productId, int customerId);
        List<int> GetWishlistProductIds(int customerId);
        List<Bracelet> GetWishlistProducts(int customerId);
    }
}
