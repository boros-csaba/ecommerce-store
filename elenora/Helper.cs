using elenora.Models;
using elenora.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace elenora
{
    public static class Helper
    {
        public static string GetFormattedMoney(decimal amount)
        {
            if (Math.Round(amount) == 0) return "0";
            return string.Format("{0:# ### ###}", Math.Round(amount));
        }

        public static string GetFormattedBraceletSize(BraceletSizeEnum? braceletSize, BraceletSizeEnum? braceletSize2)
        {
            if (braceletSize == null) return string.Empty;
            var size1 = GetFormattedBraceletSize(braceletSize.Value);
            if (braceletSize2 == null)
            {
                return string.Format("Méret: {0}", size1);
            }
            else
            {
                var size2 = GetFormattedBraceletSize(braceletSize2.Value);
                return string.Format("Méret: Férfi {0} / Női: {1}", size1, size2);
            }
        }

        private static string GetFormattedBraceletSize(BraceletSizeEnum braceletSize)
        {
            return braceletSize switch
            {
                BraceletSizeEnum.XS => "XS (14 cm)",
                BraceletSizeEnum.S => "S (15-16 cm)",
                BraceletSizeEnum.M => "M (17-18 cm)",
                BraceletSizeEnum.L => "L (19 cm)",
                BraceletSizeEnum.XL => "XL (20 cm)",
                BraceletSizeEnum.XXL => "XXL (21 cm)",
                _ => string.Empty,
            };
        }

        public static string GetFormattedShippingMethod(ShippingMethodEnum shippingMethod)
        {
            return shippingMethod switch
            {
                ShippingMethodEnum.GlsCsomagpont => "GLS csomagpont",
                ShippingMethodEnum.GLS => "GLS futárszolgálat",
                _ => string.Empty,
            };
        }

        public static string GetFormattedPaymentMethod(PaymentMethodEnum paymentMethod)
        {
            return paymentMethod switch
            {
                PaymentMethodEnum.BankTransfer => "Banki átutalás",
                PaymentMethodEnum.Barion => "Bankkártyás fizetés (Barion)",
                _ => string.Empty,
            };
        }

        public static string GetWithNonBreakingSpaces(string input)
        {
            return input.Replace(" ", "&nbsp;");
        }

        public static string GetJpgImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return imageUrl;
            if (imageUrl.EndsWith(".webp"))
            {
                return imageUrl.Replace(".webp", ".jpg");
            }
            else return imageUrl + ".jpg";
        }

        public static string GetPngImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return imageUrl;
            if (imageUrl.EndsWith(".webp"))
            {
                return imageUrl.Replace(".webp", ".png");
            }
            else return imageUrl + ".png";
        }

        public static string GetWebPImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return imageUrl;
            if (imageUrl.EndsWith(".webp"))
            {
                return imageUrl;
            }
            else return imageUrl + ".webp";
        }

        public static string GetEmailTemplate()
        {
            return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head><meta http-equiv=""Content-Type""	content=""text/html; charset=UTF-8"" /><meta name=""viewport""	content=""width=device-width, initial-scale=1.0""/><link href=""https://fonts.googleapis.com/css?family=Montserrat|Open+Sans&display=swap""	rel=""stylesheet""></head><body style=""margin: 0; padding: 0; font-size: 0.85rem""><table border=""0""	border=""0""	cellpadding=""0""	cellspacing=""0""	width=""100%""	style=""font-family: 'Open Sans', Helvetica, Arial, sans-serif; font-size: 0.85rem""><tr><td style=""padding: 20px 0 20px 0""><table align=""center""	cellpadding=""0""	cellspacing=""0""	width=""600""	style=""border-collapse: collaps; background: white""><tr><td colspan=""2""	align=""center""	style=""padding: 10px 0 30px 0""><img src=""https://www.elenora.hu/images/logo.png""	width=""200""	alt=""Elenora""	style=""display: block;""  /></td></tr>[EMAIL-CONTENT]<tr><td colspan=""2"" align=""center"" style=""padding: 60px 0 50px 0; font-family:'Montserrat', sans-serif; font-size: 1.3rem; font-weight: bold;"">Köszönjük, hogy minket választottál!</td></tr><tr><td colspan=""2"" align=""center"" style=""background: rgb(51, 51, 51); padding: 20px""><a href=""https://www.facebook.com/elenora.ekszer/"" target=""_blank""><img width=""30"" src=""https://www.elenora.hu/images/facebook.png"" alt=""Facebook"" /></a><a href=""https://www.instagram.com/elenora.ekszer/"" target=""_blank""><img width=""30"" style=""margin: 0 10px"" src=""https://www.elenora.hu/images/instagram.png"" alt=""Instagram"" /></a><a href=""https://hu.pinterest.com/elenora_ekszer/"" target=""_blank""><img width=""30"" src=""https://www.elenora.hu/images/pinterest.png"" alt=""Pinterest"" /></a></td></tr><tr><td colspan=""2"" align=""center"" style=""border-width: 1px 0 0 0; border-style: solid; border-color: #555555; font-size: 0.8rem; background: rgb(51, 51, 51); color: white"">Kapcsolat: +36 20 426 4445, info@elenora.hu<br/>1193, Budapest, Szigligeti utca 1, 9/26</td></tr></table></td></tr></table></body></html>";
        }

        public static string GetEmailTitleRow(string content)
        {
            return @$"<tr><td colspan=""2"" align=""center"" style=""padding: 10px 0 20px 0; font-family:'Montserrat', sans-serif; font-size: 1.3rem; font-weight: bold;"">{content}</td></tr>";
        }

        public static string GetEmailTextRow(string content)
        {
            return @$"<tr><td colspan=""2"" style=""padding: 10px 0 10px 0"">{content}</td></tr>";
        }

        public static string GetEmailButton(string text, string url)
        {
            return @$"<tr><td colspan=""2"" align=""center""><a href=""{url}"" style=""padding: 15px 30px 15px 30px;background:#333333;color:white !important;text-decoration:none !important"">{text}</a></td></tr>";
        }

        public static string GetProductInfoJson(List<CartItemViewModel> cartItems)
        {
            var jsonObjects = new List<string>();
            foreach (var cartItem in cartItems)
            {
                var price = cartItem.UnitPrice;
                foreach (var complementaryPorudct in cartItem.ComplementaryProducts)
                {
                    jsonObjects.Add(@$"
                    {{
                        id: 'complementary-product-{complementaryPorudct.Id}',
                        name: '{complementaryPorudct.Name}',
                        category: 'Kiegészítő termék',
                        price: '{complementaryPorudct.Price}',
                        quantity: '{cartItem.Quantity}',
                        variant: ''
                    }}");
                    price -= complementaryPorudct.Price;
                }
                jsonObjects.Add(@$"
                {{
                    id: '{cartItem.ProductIdString}',
                    item_id: '{cartItem.ProductIdString}',
                    name: '{(cartItem.IsCustomBracelet ? "Egyedi karkötő" : cartItem.ProductName)}',
                    item_name: '{(cartItem.IsCustomBracelet ? "Egyedi karkötő" : cartItem.ProductName)}',
                    category: '{cartItem.ProductCategory}',
                    price: '{price}',
                    quantity: '{cartItem.Quantity}',
                    variant: '{cartItem.Variant ?? string.Empty}'
                }}");
            }
            return $"{string.Join(", ", jsonObjects)}";
        }

        public static DateTime Now
        {
            get
            {
                try
                {
                    return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Europe/Budapest");
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Europe Standard Time");
                }
                
            }
        }

        public static string GetString(string key, string website)
        {
            if (website.EndsWith("RO"))
            {
                var value = StringsRO.ResourceManager.GetString(key);
                if (value == null)
                {
                    return key;
                }
            }
            return Strings.ResourceManager.GetString(key);
        }
    }
}
