using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
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

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
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
                    Program.form.txtProgLog_Update(ex.Message);
                    return new IRCResponse(ResponseType.Say, "Nothing found at " + match.Value, message.ReplyTo);
                }
                catch (System.UriFormatException ex)
                {
                    return null;
                }

                string title;
                match = Regex.Match(webPage.Page, @"<\s*title\s*>(.*?)</title\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    title = "Title: " + match.Groups[1].Value.Trim();
                    title = Regex.Replace(title, @"(\r|\n|)", "", RegexOptions.IgnoreCase);
                }
                else
                {
                    title = "No title found";
                }

                return new IRCResponse(ResponseType.Say, title + " (at " + webPage.Domain + ")", message.ReplyTo);
            }
            return null;
        }
    }
}
