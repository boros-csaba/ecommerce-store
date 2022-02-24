using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductIdString { get; set; }
        public string ProductName { get; set; }
        public string ProductImgageUrl { get; set; }
        public BraceletSizeEnum? BraceletSize { get; set; }
        public BraceletSizeEnum? BraceletSize2 { get; set; }
        public ProductTypeEnum ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? OriginalUnitPrice { get; set; }
        public bool IsCustomBracelet { get;set; }
        public bool IsStringBracelet { get; set; }
        public StringBraceletViewModel StringBracelet { get; set; }
        public string Timestamp { get; set; }
        public string ProductCategory { get; set; }
        public string Variant { get; set; }
        public decimal? TotalOriginalPrice
        {
            get
            {
                return OriginalUnitPrice * Quantity;
            }
        }
        public decimal TotalPrice
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
        public List<ComplementaryProductCartItemViewModel> ComplementaryProducts { get; set; } = new List<ComplementaryProductCartItemViewModel>();
    }
}
