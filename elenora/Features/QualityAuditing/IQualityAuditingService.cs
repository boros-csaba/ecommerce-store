using elenora.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.QualityAuditing
{
    public interface IQualityAuditingService
    {
        public Dictionary<string, List<ProductQualityIssue>> GetProductQualityIssues(List<Bracelet> products);
    }
}
