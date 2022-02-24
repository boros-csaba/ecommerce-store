using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IPdfService
    {
        public byte[] GeneratePdf(string html);
    }
}
