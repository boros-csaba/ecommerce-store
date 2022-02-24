using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class CartItemModel
    {
        public CartItemModel(CartItem cartItem)
        {
            Id = cartItem.Id;
            Name = cartItem.Name;
            if (cartItem is BraceletCartItem braceletCartItem)
            {
                ImageUrl = braceletCartItem.Product.MainImage;
            }
            UnitPrice = cartItem.ItemPrice;
            Quantity = cartItem.Quantity;
            Price = Quantity * UnitPrice;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
