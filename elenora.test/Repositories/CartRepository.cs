using elenora.Features.Cart;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elenora.test.Repositories
{
    class CartRepository : ICartRepository
    {
        private List<Cart> Carts { get; set; } = new List<Cart>();

        public Cart GetCustomerCart(int customerId)
        {
            return Carts.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void Add(Cart cart)
        {
            Carts.Add(cart);
        }
    }
}
