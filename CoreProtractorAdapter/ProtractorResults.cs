using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtractorTestAdapter
{

   
    public class ProtractorResult
    {
        public string description { get; set; }
        public Assertion[] assertions { get; set; }
        public int duration { get; set; }
    }

    public class Assertion
    {
        public bool passed { get; set; }
        public string errorMsg { get; set; }
        public string stackTrace { get; set; }
    }

}
