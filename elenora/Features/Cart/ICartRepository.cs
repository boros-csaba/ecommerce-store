using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.Cart
{
    public interface ICartRepository
    {
        Models.Cart GetCustomerCart(int customerId);
    }
}
