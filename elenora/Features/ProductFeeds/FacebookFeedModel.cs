using elenora.BusinessModels;
using elenora.Features.ProductPricing;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class FacebookFeedModel: IFeedModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Availability { get; set; }
        public string Condition => "new";
        public string Price { get; set; }
        public string Link { get; set; }
        public string Image_Link { get; set; }
        public string Brand => "Elenora";
        public int Fb_Product_Category => 327;
        public string Google_Product_Category => "Apparel & Accessories > Jewelry > Bracelets";
        public string Sale_Price { get; set; }
        public string Sale_Price_Effective_Date { get; set; } //todo
        public string Additional_Image_Link { get; set; }
        public string Error { get; set; }

        public FacebookFeedModel(Bracelet bracelet)
        {
            Id = bracelet.IdString;
            Title = bracelet.Name;
            Description = string.IsNullOrWhiteSpace(bracelet.ShortDescription) ? bracelet.Name : bracelet.ShortDescription;
            Availability = "in stock";
            if (bracelet.ProductComponents.Any(pc => pc.Component.ImagesQuality == 0) ||
                bracelet.SoldOut)
            {
                Availability = "out of stock";
            }
            Price = $"{bracelet.Price.Price} HUF";
            Link = "https://www.elenora.hu/karkoto/" + bracelet.IdString;

            var imageVersion = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            Image_Link = "https://www.elenora.hu/images/products/" + bracelet.IdString + "/" + bracelet.IdString + "-2048.jpg?v=" + imageVersion;
            Additional_Image_Link = "https://www.elenora.hu/images/products/" + bracelet.IdString + "/" + bracelet.IdString + "-2-2048.jpg?v=" + imageVersion;

            Validate();
        }

        public FacebookFeedModel(string type)
        {
            if (type == "custom-bracelet")
            {
                Id = "custom-bracelet";
                Title = "Egyedi karkötő";
                Description = "Egyedi karkötő";
                Availability = "in stock";
                Price = "6990 HUF";
                Link = "https://www.elenora.hu/egyedi-karkoto-keszito";
                Image_Link = "https://www.elenora.hu/images/custom-bracelet-banner-tall.png";
                Additional_Image_Link = "";
            }
        }

        public string GetHeader()
        {
            return "id,title,description,availability,condition,price,link,image_link,brand,fb_product_category,google_product_category,sale_price,sale_price_effective_date,additional_image_link";
        }

        public string GetContent()
        {
            return $@"""{Id}"",""{Title}"",""{Description}"",""{Availability}"",""{Condition}"",""{Price}"",""{Link}"",""{Image_Link}"",""{Brand}"",""{Fb_Product_Category}"",""{Google_Product_Category}"",""{Sale_Price}"",""{Sale_Price_Effective_Date}"",""{Additional_Image_Link}""";
        }

        private void Validate()
        {
            if (Id.Length >= 100) Error = "Id character limit 100 exceeded!";
            if (Title.Length > 150) Error = "Title character limit 150 exceeded!";
        }
    }
}
