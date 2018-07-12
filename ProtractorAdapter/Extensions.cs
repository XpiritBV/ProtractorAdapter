using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProtractorTestAdapter
{
    public static class Extensions
    {
        public static bool Like(this string str, string pattern) => DotNet.Globbing.Glob.Parse(pattern).IsMatch(str);
    }
}
