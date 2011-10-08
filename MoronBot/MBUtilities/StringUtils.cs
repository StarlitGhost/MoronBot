using System.Text.RegularExpressions;

namespace MBUtilities
{
    public static class StringUtils
    {
        public static string ReplaceNewlines(string text, string replacement)
        {
            return Regex.Replace(text, @"[\r\n]+", replacement);
        }

        public static string StripIRCFormatChars(string text)
        {
            return Regex.Replace(text, @"(\x03([0-9]+)?(,[0-9]+)?|\x02|\x16|\x1F|\x0F)", string.Empty);
        }
    }
}
