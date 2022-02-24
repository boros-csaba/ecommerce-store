using elenora.BusinessModels;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.QualityAuditing
{
    public class QualityAuditingService : IQualityAuditingService
    {
        private readonly IWebHostEnvironment environment;

        public QualityAuditingService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public Dictionary<string, List<ProductQualityIssue>> GetProductQualityIssues(List<Bracelet> products)
        {
            var result = new Dictionary<string, List<ProductQualityIssue>>
            {
                { "Egyetlen gyöngynek sincs leírása", GetBraceletsWithoutComponentInformation(products) },
                { "Nincs 360 fokos videó", GetBraceletsWithout360Video(products) }
            };
            return result;
        }

        private List<ProductQualityIssue> GetBraceletsWithout360Video(List<Bracelet> products)
        {
            return products.Where(
                p => !File.Exists(Path.Combine(environment.WebRootPath, "images", "products", p.IdString, $"{p.IdString}-360.mp4")))
                .Select(p => new ProductQualityIssue
                {
                    Product = p
                }).ToList();
        }

        private List<ProductQualityIssue> GetBraceletsWithoutComponentInformation(List<Bracelet> products)
        {
            return products.Where(
                p => p.ProductComponents.All(
                    pc => string.IsNullOrWhiteSpace(pc.Component.ComponentFamily?.Description) ||
                          string.IsNullOrWhiteSpace(pc.Component.ImageUrl)))
                .Select(p => new ProductQualityIssue
                {
                    Product = p
                }).ToList();
        }
    }
}
