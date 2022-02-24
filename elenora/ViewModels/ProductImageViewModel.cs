using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class ProductImageViewModel
    {
        public string IdString { get; set; }
        public string MainImageUrl { get; set; }
        public string AltText { get; set; }
        public string PictureClass { get; set; }
        public string ImageClass { get; set; }
        public string Sizes { get; set; }
        public int? ImageNr { get; set; }

        public ProductImageViewModel(ProductViewModel product, string pictureClass, string imageClass, string sizes, int? imageNr = null)
        {
            IdString = product.IdString;
            MainImageUrl = product.MainImageUrl;
            AltText = product.Name;
            PictureClass = pictureClass;
            ImageClass = imageClass;
            Sizes = sizes;
            ImageNr = imageNr;
        }

        public ProductImageViewModel(Features.ProductList.ProductListItem product, string pictureClass, string imageClass, string sizes, int? imageNr = null)
        {
            IdString = product.ProductIdString;
            AltText = product.ProductName;
            PictureClass = pictureClass;
            ImageClass = imageClass;
            Sizes = sizes;
            ImageNr = imageNr;
        }

        public ProductImageViewModel(ProductListItemViewModel product, string pictureClass, string imageClass, string sizes)
        {
            IdString = product.IdString;
            MainImageUrl = product.MainImageUrl;
            AltText = $"{product.Name} - {product.SecondaryName}";
            PictureClass = pictureClass;
            ImageClass = imageClass;
            Sizes = sizes;
        }

        public ProductImageViewModel(CartItemViewModel cartItem, string pictureClass, string imageClass, string sizes)
        {
            IdString = cartItem.ProductIdString;
            MainImageUrl = cartItem.ProductImgageUrl;
            AltText = cartItem.ProductName;
            PictureClass = pictureClass;
            ImageClass = imageClass;
            Sizes = sizes;
        }
    }
}
