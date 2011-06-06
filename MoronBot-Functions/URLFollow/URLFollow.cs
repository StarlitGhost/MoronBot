using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Automatic
{
    public class URLFollow : Function
    {
        public URLFollow()
        {
            Help = "Automatic function that follows posted urls and grabs the domain and title of the resultant webpage, then posts them to the chat.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            Match match = Regex.Match(message.MessageString, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                if (Regex.IsMatch(match.Value, @"\.(jpe?g|gif|png|bmp)$"))
                {
                    return null;
                }

                URL.WebPage webPage;
                try
                {
                    webPage = URL.FetchURL(match.Value);
                }
                catch (System.Net.WebException ex)
                {
                    // Nothing returned when attempting to fetch the url
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Nothing found at " + match.Value, message.ReplyTo) };
                }
                catch (System.UriFormatException ex)
                {
                    // Invalid url detected, don't really care though.
                    return null;
                }

                if (webPage.Domain == null)
                    return null;

                // Hunt for the title tags on the page, and grab the text between them.
                string title;
                match = Regex.Match(webPage.Page, @"<\s*title\s*>(.*?)</title\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                // Title tags found
                if (match.Success)
                {
                    // Trim excess whitespace
                    title = "Title: " + match.Groups[1].Value.Trim();
                    // Remove newlines
                    title = Regex.Replace(title, @"(\r|\n)", " ", RegexOptions.IgnoreCase);
                    // Reduce multiple spaces to a single space
                    title = Regex.Replace(title, @"(\s+)", " ", RegexOptions.IgnoreCase);
                    title = HttpUtility.HtmlDecode(title);
                }
                // No title tags found
                else
                {
                    title = "No title found";
                }

                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, title + " (at " + webPage.Domain + ")", message.ReplyTo) };
            }
            return null;
        }
    }
}
