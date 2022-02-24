using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class PinterestFeedModel : IFeedModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Image_link { get; set; }
        public string Price { get; set; }
        public string Availability { get; set; }
        public string Additional_image_link { get; set; }

        public PinterestFeedModel(Bracelet product)
        {
            Id = product.Id;
            Title = product.Name;
            Description = "Ásványkarkötő";
            Availability = "in stock";
            if (!Title.Contains("karkötő")) Title = "Ásványkarkötő: " + Title;
            Link = "https://www.elenora.hu/karkoto/" + product.IdString;
            var imageVersion = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            Image_link = "https://www.elenora.hu/images/products/" + product.IdString + "/" + product.IdString + "-2048.jpg?v=" + imageVersion;
            Additional_image_link = "https://www.elenora.hu/images/products/" + product.IdString + "/" + product.IdString + "-2-2048.jpg?v=" + imageVersion;
            Price = $"{product.Price.Price} HUF";
        }

        public string GetContent()
        {
            return $"{Id}\t{Availability}\t{Description}\t{Image_link}\t{Link}\t{Price}\t{Title}\t{Additional_image_link}\tElenora";
            
        }

        public string GetHeader()
        {
            return "id\tavailability\tdescription\timage_link\tlink\tprice\ttitle\tadditional_image_link\tbrand";
        }
    }
}
