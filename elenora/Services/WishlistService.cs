using elenora.BusinessModels;
using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly DataContext context;
        private readonly IProductService productService;

        public WishlistService(DataContext context, IProductService productService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public void AddToWishlist(int productId, int customerId)
        {
            var item = context.WishlistItems.FirstOrDefault(i => i.ProductId == productId && i.CustomerId == customerId);
            if (item == null)
            {
                item = new WishlistItem
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    AddedDate = Helper.Now
                };
                context.WishlistItems.Add(item);
                context.SaveChanges();
            }
        }

        public void DeleteFromWishlist(int productId, int customerId)
        {
            var item = context.WishlistItems.FirstOrDefault(i => i.ProductId == productId && i.CustomerId == customerId);
            if (item != null)
            {
                context.WishlistItems.Remove(item);
                context.SaveChanges();
            }
        }

        public List<int> GetWishlistProductIds(int customerId)
        {
            return context.WishlistItems.Where(i => i.CustomerId == customerId).Select(i => i.ProductId).ToList();
        }

        public List<Bracelet> GetWishlistProducts(int customerId)
        {
            var productIds = GetWishlistProductIds(customerId);
            return productService.GetActiveProducts(p => productIds.Contains(p.Id));
        }
    }
}
