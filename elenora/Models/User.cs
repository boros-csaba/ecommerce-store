using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class User: IdentityUser<int>
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
