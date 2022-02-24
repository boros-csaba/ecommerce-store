using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.QualityAuditing
{
    public class ProductQualityIssue
    {
        public Bracelet Product { get; set; }
        public string Information { get; set; }
    }
}
