using elenora.Features.HoroscopeBracelets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class HoroscopePageViewModel
    {
        public List<HoroscopeViewModel> HoroscopeBracelets { get; set; }
        public List<HoroscopeViewModel> Horoscopes { get; set; }
        public List<HoroscopeListSectionViewModel> Sections { get; set; }
    }
}
