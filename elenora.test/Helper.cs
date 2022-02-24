using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace elenora.test
{
    public static class Helper
    {
        public static int CreateProduct(DataContext context, decimal price = 5490)
        {
            var product = new Bracelet
            {
                //Price = price
            };
            context.Products.Add(product);
            context.SaveChanges();
            return product.Id;
        }

        public static void CreatePercentageCoupon(DataContext context, int percentage, string code)
        {
            var coupon = new Coupon
            {
                Percentage = percentage,
                Code = code
            };
            context.Coupons.Add(coupon);
            context.SaveChanges();
        }
    }
}
