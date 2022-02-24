using elenora.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.HoroscopeBracelets
{
    public class HoroscopeListSectionViewModel
    {
        public string HoroscopeName { get; set; }
        public string HoroscopeIdString { get; set; }
        public string SectionButtonText
        {
            get
            {
                var suffix = "a";
                if (HoroscopeName.StartsWith("I") || HoroscopeName.StartsWith("O")) suffix = "az";
                return $"Tovább {suffix} {HoroscopeName} karkötőkhöz";
            }
        }
        public List<ProductListItemViewModel> Products { get; set; }
    }
}
