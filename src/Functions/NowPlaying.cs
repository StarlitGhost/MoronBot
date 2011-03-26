using System.Text.RegularExpressions;

using CwIRC;
using Bitly;

namespace MoronBot.Functions
{
    class NowPlaying : Function
    {
        public NowPlaying(MoronBot moronBot)
        {
            Name = GetName();
            Help = "nowplaying/np (<user>)\t\t- Returns your currently playing music (from Last.fm). You can also supply a specific username to check.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(n(ow)?p(laying)?)$", RegexOptions.IgnoreCase))
            {
                string lastfmName = "";

                if (message.ParameterList.Count > 0)
                {
                    lastfmName = message.ParameterList[0];
                }
                else
                {
                    lastfmName = message.User.Name;
                }

                Utilities.URL.WebPage recentFeed = new Utilities.URL.WebPage();

                try
                {
                    recentFeed = Utilities.URL.FetchURL("http://ws.audioscrobbler.com/1.0/user/" + lastfmName + "/recenttracks.rss");
                }
                catch (System.Net.WebException ex)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "User \"" + lastfmName + "\" not found on Last.fm", message.ReplyTo));
                    return;
                }

                Match track = Regex.Match(recentFeed.Page, @"<item>\s*?<title>(?<band>.+?)–(?<song>.+?)</title>\s*?<link>(?<link>.+?)</link>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                string band = track.Groups["band"].Value;
                string song = track.Groups["song"].Value;
                string link = Utilities.URL.Shorten(track.Groups["link"].Value);

                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "\"" + song.Trim() + "\" by " + band.Trim() + " (" + link + ")", message.ReplyTo));
                return;
            }
            else
            {
                return;
            }
        }
    }
}
