using System.Text.RegularExpressions;

namespace MBUtilities
{
    public static class StringUtils
    {
        public static string ReplaceNewlines(string text, string replacement)
        {
            return Regex.Replace(text, @"[\r\n]+", replacement);
        }
    }
}
