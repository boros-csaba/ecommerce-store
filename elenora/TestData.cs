using elenora.BusinessModels;
using elenora.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora
{
    public static class TestData
    {
        public static void GenerateTestData(DataContext context)
        {
            context.Categories.Add(new Category { Id = 1, Name = "Női karkötők", IdString = "noi-karkotok" });
            context.Categories.Add(new Category { Id = 2, Name = "Férfi karkötők", IdString = "ferfi-karkotok" });
            context.Categories.Add(new Category { Id = 3, Name = "Páros karkötők", IdString = "paros-karkotok" });
            context.Categories.Add(new Category { Id = 4, Name = "Horoszkóp karkötők", IdString = "horoszkop-karkotok" });
            context.Categories.Add(new Category { Id = 5, Name = "Csakra karkötők", IdString = "csakra-karkotok" });

            AddProducts(context);
            SetupBraceletDesigner(context);

            context.Coupons.Add(new Coupon { Id = 1, Code = "ELSO100", Name = "20% az első 100 vásárlónknak", MaxUsageCount = 100, StartDate = new DateTime(2020, 1, 1), Percentage = 20, UsageCount = 0 });

            context.SaveChanges();
        }

        private static void AddProducts(DataContext context)
        {
            var product1 = new Bracelet
            {
                Id = 1,
                Name = "Timoti",
                IdString = "tomiti-regalit",
                ProductType = ProductTypeEnum.Bracelet,
                ListOrder = 1,
                State = ProductStateEnum.Active,
                MainImage = "/images/products/timoti-turkiz-regalit-csaszarko/timoti-turkiz-regalit-csaszarko-600.webp",
                CatalogImage1 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/timoti-1.webp",
                CatalogImage2 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/timoti-2.webp",
                ShortDescription = "A karkötő regalit ásványkőből készül. A regalit elnyeli a negatív energiákat, kitartóbbá teszi viselőjét és segít céljai megvalósítását. Bátorságot ad, tisztítja és összehangolja a csakrákat.",
                CategoryId = 1,
                //Price = 4990
            };
            context.Products.Add(product1);

            var product2 = new Bracelet
            {
                Id = 2,
                Name = "Our World",
                IdString = "our-world-regalit-csaszarko",
                ProductType = ProductTypeEnum.BraceletPair,
                ListOrder = 2,
                State = ProductStateEnum.Active,
                MainImage = "/images/products/our-world-regalit-csaszarko.webp",
                CatalogImage1 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/our-world-regalit-csaszarko-1.webp",
                CatalogImage2 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/our-world-regalit-csaszarko-2.webp",
                ShortDescription = "A karkötők regalit ásványkőből és matt ónixból készülnek. A regalit elnyeli a negatív energiákat, kitartóbbá teszi viselőjét és segít céljai megvalósítását. Bátorságot ad, tisztítja és összehangolja a csakrákat.",
                CategoryId = 3,
                //Price = 7990
            };
            context.Products.Add(product2);

            var product3 = new Bracelet
            {
                Id = 3,
                Name = "Amelia",
                IdString = "amelia-hegyikristaly-jade",
                ProductType = ProductTypeEnum.Bracelet,
                ListOrder = 3,
                State = ProductStateEnum.Active,
                MainImage = "/images/products/amelia-hegyikristaly-jade.webp",
                CatalogImage1 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/amelia-1.webp",
                CatalogImage2 = "https://elenora.s3.eu-central-1.amazonaws.com/product-images/amelia-2.webp",
                ShortDescription = "A karkötő roppantott hegyikristály és fehér jáde ásványkövekből készül. A hegyikristály erősíti az összes többi kő erejét, tisztítja az aurát. A Jáde vonzza a szerencsét, megszünteti az ingerlékenységet.",
                CategoryId = 1,
                //Price = 5490,
                //OriginalPrice = 6990
            };
            context.Products.Add(product3);
        }

        private static void SetupBraceletDesigner(DataContext context)
        {
            var rozsakvarcFamily = new ComponentFamily
            {
                Name = "Rózsakvarc",
                Description = "Description",
                ShortDescription = "ShortDescription"
            };
            context.ComponentFamilies.Add(rozsakvarcFamily);

            var rozsakvarc = new Component
            {
                Name = "Rózsakvarc",
                ImageUrl = "/images/components/rozsakvarc.webp",
                SelectabeInBraceletDesigner = true,
                ComponentFamily = rozsakvarcFamily,
            };
            context.Components.Add(rozsakvarc);

            var feherA = new Component
            {
                Id = 27,
                Name = "Fehér A",
                ImageUrl = "/images/components/feher-a.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherA);

            var feherE = new Component
            {
                Id = 31,
                Name = "Fehér E",
                ImageUrl = "/images/components/feher-e.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherE);

            var feherL = new Component
            {
                Id = 38,
                Name = "Fehér L",
                ImageUrl = "/images/components/feher-l.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherL);

            var feherN = new Component
            {
                Id = 40,
                Name = "Fehér N",
                ImageUrl = "/images/components/feher-n.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherN);

            var feherO = new Component
            {
                Id = 41,
                Name = "Fehér O",
                ImageUrl = "/images/components/feher-o.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherO);

            var feherR = new Component
            {
                Id = 44,
                Name = "Fehér R",
                ImageUrl = "/images/components/feher-r.webp",
                HeightRatio = 1,
                WidthRatio = 0.75f
            };
            context.Components.Add(feherR);

            var csomoRejto = new Component
            {
                Id = 115,
                Name = "Csomó rejtő",
                ImageUrl = "/images/components/csomofogo.webp",
                HeightRatio = 2.33f,
                WidthRatio = 0.26f
            };
            context.Components.Add(csomoRejto);

            var feketeKorong = new Component
            {
                Id = 129,
                Name = "Fekete korong",
                ImageUrl = "/images/components/fekete-korong.webp",
                HeightRatio = 2.33f,
                WidthRatio = 0.26f
            };
            context.Components.Add(feketeKorong);

            var complementaryProduct = new ComplementaryProduct
            {
                Name = "Test product",
                Price = 1000,
                ImageUrl = "/images/products/tigrisszem-kiegeszito-karkoto.webp"
            };
            context.ComplementaryProducts.Add(complementaryProduct);

            var beadComplementaryProduct = new BeadComplementaryProduct
            {
                ComplementaryProduct = complementaryProduct,
                Component = rozsakvarc
            };
            context.BeadComplementaryProducts.Add(beadComplementaryProduct);
        }
    }
}
