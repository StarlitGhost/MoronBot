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
                if (message.ParameterList.Count > 0)
                {
                    return;
                }
                else
                {
                    Utilities.URL.WebPage recentFeed = Utilities.URL.FetchURL("http://ws.audioscrobbler.com/1.0/user/" + message.User.Name + "/recenttracks.rss");

                    Match track = Regex.Match(recentFeed.Page, @"<item>.+?<title>(.+?)</title>.+?<link>(.+?)</link>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    string name = track.Groups[1].Value;
                    string link = Utilities.URL.Shorten(track.Groups[2].Value);

                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, name + " (" + link + ")", message.ReplyTo));

                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
