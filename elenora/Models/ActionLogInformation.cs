using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class ActionLogInformation
    {
        public string IpAddress { get; set; }
        public string DeviceType { get; set; }
        public bool IsBot { get; set; }
        public string Referrer { get; set; }
    }
}
