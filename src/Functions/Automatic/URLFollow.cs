using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Automatic
{
    class URLFollow : Function
    {
        public URLFollow(MoronBot moronBot)
        {
            Name = GetName();
            Help = "Automatic function that follows posted urls and grabs the domain and title of the resultant webpage, then posts them to the chat.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            Match match = Regex.Match(message.MessageString, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                Utilities.URL.WebPage webPage;
                try
                {
                    webPage = Utilities.URL.FetchURL(match.Value);
                }
                catch (System.Net.WebException ex)
                {
                    // Nothing returned when attempting to fetch the url
                    Program.form.txtProgLog_Update(ex.Message);
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Nothing found at " + match.Value, message.ReplyTo));
                    return;
                }
                catch (System.UriFormatException ex)
                {
                    // Invalid url detected, don't really care though.
                    return;
                }

                // Hunt for the title tags on the page, and grab the text between them.
                string title;
                match = Regex.Match(webPage.Page, @"<\s*title\s*>(.*?)</title\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                // Title tags found
                if (match.Success)
                {
                    // Trim excess whitespace
                    title = "Title: " + match.Groups[1].Value.Trim();
                    // Remove newlines
                    title = Regex.Replace(title, @"(\r|\n|)", "", RegexOptions.IgnoreCase);
                }
                // No title tags found
                else
                {
                    title = "No title found";
                }

                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, title + " (at " + webPage.Domain + ")", message.ReplyTo));
                return;
            }
            return;
        }
    }
}
