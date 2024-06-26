using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoudnessMeter.Extentions
{
    public static class StringExtensions
    {
        public static string SplitUpperCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text, "(?<!^)([A-Z])", " $1");
        }
    }
}
