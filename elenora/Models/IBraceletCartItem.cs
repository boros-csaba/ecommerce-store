using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public interface IBraceletWithSize
    {
        BraceletSizeEnum? BraceletSize { get; set; }
    }
}
