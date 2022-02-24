using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels
{
    public class HoroscopeViewModel
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string DateRange { get; set; }
        public List<HoroscopeBeadViewModel> Beads { get; set; }

        public HoroscopeViewModel()
        {

        }

        public HoroscopeViewModel(Horoscope horoscope)
        {
            Id = horoscope.Id;
            IdString = horoscope.IdString;
            Name = horoscope.Name;
            DateRange = horoscope.DateRange;
        }
    }
}
