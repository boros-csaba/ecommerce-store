using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;

namespace elenora.Services
{
    public class PdfService : IPdfService
    {
        private readonly IGeneratePdf generatePdf;

        public PdfService(IGeneratePdf generatePdf)
        {
            this.generatePdf = generatePdf ?? throw new ArgumentNullException(nameof(generatePdf));
        }

        public byte[] GeneratePdf(string html)
        {
            return generatePdf.GetPDF(html);
        }
    }
}
