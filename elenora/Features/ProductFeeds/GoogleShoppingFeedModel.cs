using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class GoogleShoppingFeedModel : IFeedModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Image_link { get; set; }
        public string Additional_image_link { get; set; }
        public string Availability { get; set; }
        public string Price { get; set; }
        public string Sale_price { get; set; }
        public string Sale_price_effective_date { get; set; }
        public string Google_product_category { get; set; }
        public string Brand { get; set; }
        public string Identifier_exists { get; set; }
        public string Material { get; set; }

        public GoogleShoppingFeedModel(Bracelet product)
        {
            Id = product.Id;
            Title = product.Name;
            Description = "Ásványkarkötő";
            if (!Title.Contains("karkötő")) Title = "Ásványkarkötő: " + Title;
            Link = "https://www.elenora.hu/karkoto/" + product.IdString;
            var imageVersion = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            Image_link = "https://www.elenora.hu/images/products/" + product.IdString + "/" + product.IdString + "-2048.jpg?v=" + imageVersion;
            Additional_image_link = "https://www.elenora.hu/images/products/" + product.IdString + "/" + product.IdString + "-2-2048.jpg?v=" + imageVersion;
            Price = $"{product.Price.Price} HUF";
        }

        public string GetContent()
        {
            return $"{Id}\t{Title}\t{Description}\t{Link}\t{Image_link}\t{Additional_image_link}\tin stock\t{Price}\tApparel & Accessories > Jewelry > Bracelets\tElenora\tno";
        }

        public string GetHeader()
        {
            return "id\ttitle\tdescription\tlink\timage_link\tadditional_image_link\tavailability\tprice\tgoogle_product_category\tbrand\tidentifier_exists";
        }
    }
}
