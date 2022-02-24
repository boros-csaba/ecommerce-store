using elenora.Features.ProductFeeds;
using elenora.Features.ProductPricing;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace elenora.BusinessModels
{
    [Table("Products")]
    public class Bracelet
    {
        [Key]
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        [Column("Price")]
        public decimal BasePrice { get; set; }
        public bool Featured { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ProductTypeEnum ProductType { get; set; }
        public int ListOrder { get; set; }
        public ProductStateEnum State { get; set; }
        public string MainImage { get; set; }
        public string CatalogImage1 { get; set; }
        public string CatalogImage2 { get; set; }
        public string ShortDescription { get; set; }
        public string HtmlDescription { get; set; }
        public List<ProductComponent> ProductComponents { get; set; }
        [NotMapped]
        public ProductPrice Price { get; set; }
        [NotMapped]
        public bool SoldOut { get; set; }
        public List<ProductStatistics> ProductStatistics { get; set; }
        public List<ProductDiscount> ProductDiscounts { get; set; }
        public List<ProductFeedVisibility> ProductFeedVisibilities { get; set; }

        public string Description
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ShortDescription))
                {
                    return ShortDescription;
                }
                return null;
            }
        }
    }
}
