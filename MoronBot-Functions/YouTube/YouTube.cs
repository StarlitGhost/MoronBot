using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    /// <summary>
    /// A Function which returns the title, length, description, and link of the top YouTube search result for the given search term.
    /// </summary>
    /// <suggester>SirGir</suggester>
    public class YouTube : Function
    {
        public YouTube()
        {
            Help = "youtube <terms> - Gives you the first YouTube search result for a given search term.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(youtube)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Search for what?", message.ReplyTo);
                return;
            }

            string query = HttpUtility.UrlEncode(message.Parameters);
            string url = "https://gdata.youtube.com/feeds/api/videos?q=" + query +
                "&orderby=relevance" +
                "&max-results=1" +
                "&v=2" +
                "&key=AI39si4LaIHfBlDmxNNRIqZjXYlDgVTmUVa7p8dSE8_bI45a9leskPQKauV7qi-qmAqjf6zjTdhwAfJxOfkxNcYOmloh8B1X9Q";
                    
            URL.WebPage page;
            try
            {
                page = URL.FetchURL(url);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                FuncInterface.SendResponse(ResponseType.Say, "Couldn't fetch results from YouTube", message.ReplyTo);
                return;
            }

            Match linkMatch = Regex.Match(page.Page, @"<link rel='alternate' type='text/html' href='([^<]+?)&amp;");

            if (!linkMatch.Success)
            {
                FuncInterface.SendResponse(ResponseType.Say, "No results found for \"" + message.Parameters + "\"", message.ReplyTo);
                return;
            }

            Match titleMatch = Regex.Match(page.Page, @"<title>([^<]+?)</title><content");
            Match lengthMatch = Regex.Match(page.Page, @"<yt:duration seconds='([0-9]+?)'/>");
            Match descriptionMatch = Regex.Match(page.Page, @"<media:description type='plain'>([^<]+?)</media:description>");

            string title = HttpUtility.HtmlDecode(titleMatch.Groups[1].Value);
            TimeSpan lengthSpan = TimeSpan.FromSeconds(Int32.Parse(lengthMatch.Groups[1].Value));
            string length = (lengthSpan.Hours > 0 ? lengthSpan.Hours.ToString() + ":" : String.Empty);
            length += lengthSpan.Minutes.ToString("D2") + ":" + lengthSpan.Seconds.ToString("D2");
            string description = URL.StripHTML(HttpUtility.HtmlDecode(descriptionMatch.Groups[1].Value));
            if (description.Length > 150)
                description = description.Substring(0, 147) + "...";
            string link = linkMatch.Groups[1].Value;

            FuncInterface.SendResponse(
                ResponseType.Say,
                title + " | " + length + " | " + description + " | " + link,
                message.ReplyTo);
            return;
        }
    }
}
