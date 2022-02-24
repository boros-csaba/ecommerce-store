using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewModels.Admin
{
    public class QuizResultViewModel
    {
        public string QuizName { get; set; }
        public int? StartCount { get; set; }
        public string Answer1Text { get; set; }
        public string Answer2Text { get; set; }
        public string Answer3Text { get; set; }
        public string Answer4Text { get; set; }
        public string Answer5Text { get; set; }
        public string Answer6Text { get; set; }
        public string Result { get; set; }
    }
}
