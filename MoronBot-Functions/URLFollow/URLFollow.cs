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
using MBUtilities.Channel;

namespace Automatic
{
    public class URLFollow : Function
    {
        public URLFollow()
        {
            Help = "Automatic function that follows urls and grabs information about the resultant webpage.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (message.Type != "PRIVMSG"
                || message.TargetType != IRCMessage.TargetTypes.CHANNEL
                || ChannelList.ChannelHasMode(message.ReplyTo, 'U'))
                return null;

            Match match = Regex.Match(message.MessageString, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;
            
            if (Regex.IsMatch(match.Value, @"\.(jpe?g|gif|png|bmp)$"))
                return null;

            string response = null;

            Match youtubeMatch = Regex.Match(match.Value, @"www\.youtube\.com/watch\?v=([^&]+)");
            if (youtubeMatch.Success)
                response = FollowYouTube(youtubeMatch.Groups[1].Value);
            else
                response = FollowStandard(match.Value);

            if (response == null)
                return null;

            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, response, message.ReplyTo) };
        }

        string FollowYouTube(string videoID)
        {
            string url = "https://gdata.youtube.com/feeds/api/videos/" + videoID + "?v=2&key=AI39si4LaIHfBlDmxNNRIqZjXYlDgVTmUVa7p8dSE8_bI45a9leskPQKauV7qi-qmAqjf6zjTdhwAfJxOfkxNcYOmloh8B1X9Q";

            URL.WebPage page;
            try
            {
                page = URL.FetchURL(url);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message + "\n" + url, Settings.Instance.ErrorFile);
                return null;
            }

            Match titleMatch = Regex.Match(page.Page, @"<title>([^<]+?)</title><content");

            if (titleMatch.Success)
            {
                Match lengthMatch = Regex.Match(page.Page, @"<yt:duration seconds='([0-9]+?)'/>");
                Match descriptionMatch = Regex.Match(page.Page, @"<media:description type='plain'>([^<]+?)</media:description>");

                string title = HttpUtility.HtmlDecode(titleMatch.Groups[1].Value);
                TimeSpan lengthSpan = TimeSpan.FromSeconds( Int32.Parse(lengthMatch.Groups[1].Value) );
                string length = (lengthSpan.Hours > 0 ? lengthSpan.Hours.ToString() + ":" : String.Empty);
                length += lengthSpan.Minutes.ToString("D2") + ":" + lengthSpan.Seconds.ToString("D2");
                string description = URL.StripHTML( HttpUtility.HtmlDecode(descriptionMatch.Groups[1].Value) );
                description = StringUtils.ReplaceNewlines(description, " ");
                if (description.Length > 150)
                    description = description.Substring(0, 147) + "...";

                return title + " | " + length + " | " + description;
            }
            return null;
        }

        string FollowStandard(string url)
        {
            URL.WebPage webPage;
            webPage.Domain = null;
            webPage.Page = null;
            try
            {
                webPage = URL.FetchURL(url);
            }
            catch (System.Net.WebException ex)
            {
                Logger.Write(ex.ToString() + "\r\nURL: " + url, Settings.Instance.ErrorFile);
                // Nothing returned when attempting to fetch the url
                //return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Nothing found at " + match.Value, message.ReplyTo) };
            }
            catch (System.UriFormatException ex)
            {
                Logger.Write(ex.ToString() + "\r\nURL: " + url, Settings.Instance.ErrorFile);
                // Invalid url detected, don't really care though.
                return null;
            }

            if (webPage.Domain == null)
                return null;

            // Hunt for the title tags on the page, and grab the text between them.
            string title;
            Match match = Regex.Match(webPage.Page, @"<\s*title\s*>(.*?)</title\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // Title tags found
            if (match.Success)
            {
                // Trim excess whitespace
                title = "Title: " + match.Groups[1].Value.Trim();
                // Remove newlines
                title = Regex.Replace(title, @"(\r|\n)", " ", RegexOptions.IgnoreCase);
                // Reduce multiple spaces to a single space
                title = Regex.Replace(title, @"(\s+)", " ", RegexOptions.IgnoreCase);
                // *Hopefully* replace html character entities with their normal text counterparts
                title = HttpUtility.HtmlDecode(title);
                // Strip text-direction character entities
                title = Regex.Replace(title, @"&#x202[ac];", string.Empty, RegexOptions.IgnoreCase);
            }
            // No title tags found
            else
            {
                title = "No title found";
            }
            
            return title + " (at " + webPage.Domain + ")";
        }
    }
}
