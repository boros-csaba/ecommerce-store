using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ProductListItemViewModel
    {
        public int Id { get; private set; }
        public string IdString { get; private set; }
        public string Name { get; private set; }
        public string SecondaryName { get; private set; }
        public string ShortDescription { get; private set; }
        public decimal Price { get; private set; }
        public decimal? OriginalPrice { get; private set; }
        public string MainImageUrl { get; private set; }
        public string CatalogImage1 { get; private set; }
        public string CatalogImage2 { get; private set; }
        public bool InWishlist { get; set; }
        public ProductTypeEnum ProductType { get; private set; }
        public string ColorCode { get; private set; }
        public List<ComponentViewModel> Components { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; }
        public int DiscountPercentage
        {
            get
            {
                if (OriginalPrice == null) return 0;
                return (int)Math.Floor((OriginalPrice.Value - Price) / OriginalPrice.Value * 100);
            }
        }

        public string Url { get; set; }
        public string AddToCartAction { get; set; }
        public bool SoldOut { get; set; }

        public ProductListItemViewModel(Bracelet product)
        {
            Id = product.Id;
            IdString = product.IdString;
            Name = product.Name;
            ShortDescription = product.ShortDescription;
            Price = product.Price.Price;
            OriginalPrice = product.Price.OriginalPrice;
            MainImageUrl = product.MainImage;
            CatalogImage1 = product.CatalogImage1;
            CatalogImage2 = product.CatalogImage2;
            ProductType = product.ProductType;
            Url = $"/karkoto/{product.IdString}";
            AddToCartAction = $"return addToCart(this, {product.Id}, '{product.IdString}', '{product.Name}', '{product.Category.Name}', '{product.Price}');";
            SoldOut = product.SoldOut;
        }
    }
}
