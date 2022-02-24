using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; private set; }
        public string IdString { get; private set; }
        public string Name { get; private set; }
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
        public string CategoryName { get; private set; }
        public int DiscountPercentage
        {
            get
            {
                if (OriginalPrice == null) return 0;
                return (int)Math.Floor((OriginalPrice.Value - Price)/OriginalPrice.Value * 100);
            }
        }
        public bool ShowQuantitySelector { get; private set; }
        public string HtmlDescription { get; set; }
        public bool SoldOut { get; set; }
        public List<Promotion> Promotions { get; set; }
        public string Video360Path { get; set; }
        public string VideoHandPath { get; set; }

        public ProductViewModel(Bracelet product, string webRootPath)
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
            CategoryName = product.Category.Name;
            ShowQuantitySelector = product.ProductType != ProductTypeEnum.CustomTextBracelet && product.ProductType != ProductTypeEnum.CustomLetterBracelet;
            HtmlDescription = product.HtmlDescription;
            SoldOut = product.SoldOut;

            var video360ServerPath = Path.Combine(webRootPath, "images", "products", IdString, $"{IdString}-360.mp4");
            if (File.Exists(video360ServerPath))
            {
                Video360Path = $"/images/products/{IdString}/{IdString}-360.mp4";
            }
            var videoHandServerPath = Path.Combine(webRootPath, "images", "products", IdString, $"{IdString}.mp4");
            if (File.Exists(videoHandServerPath))
            {
                VideoHandPath = $"/images/products/{IdString}/{IdString}.mp4";
            }
        }
    }
}
