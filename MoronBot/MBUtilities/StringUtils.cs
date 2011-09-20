namespace MBUtilities
{
    public static class StringUtils
    {
        public static string ReplaceNewlines(string text, string replacement)
        {
            return text.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement);
        }
    }
}
