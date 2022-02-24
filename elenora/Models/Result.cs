using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public class Result
    {
        public Result()
        {

        }

        public Result(string error)
        {
            ErrorMessage = error;
        }

        public bool Success { get { return string.IsNullOrWhiteSpace(ErrorMessage); } }
        public string ErrorMessage { get; set; }
    }
}
