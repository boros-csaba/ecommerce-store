using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.ProductFeeds
{
    public class ArukeresoFeedModel : IFeedModel
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public string Product_url { get; set; }
        public decimal Price { get; set; }
        public decimal Delivery_Cost { get; set; }
        public string Image_url { get; set; }

        public ArukeresoFeedModel(Bracelet product)
        {
            Identifier = product.Id;
            Name = product.Name;
            if (!Name.Contains("karkötő")) Name = "Ásványkarkötő: " + Name;
            Product_url = "https://www.elenora.hu/karkoto/" + product.IdString;
            Price = product.Price.Price;
            Delivery_Cost = Settings.GLS_SHIPPING_PRICE(false);
            var imageVersion = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            Image_url = "https://www.elenora.hu/images/products/" + product.IdString + "/" + product.IdString + "-2048.jpg?v=" + imageVersion;
        }

        public string GetContent()
        {
            return $"{Identifier},Elenora,{Name},Karkötő,{Product_url},{Price},{Price},{Delivery_Cost},{Image_url},3 munkanap";
        }

        public string GetHeader()
        {
            return "Identifier,Manufacturer,Name,Category,Product_url,Price,Net_price,Delivery_Cost,Image_url,Delivery_Time";
        }
    }
}
