using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using Bitly;

namespace MoronBot.Functions
{
    class NowPlaying : Function
    {
        public static Dictionary<string, string> AccountMap = new Dictionary<string, string>();

        public NowPlaying(MoronBot moronBot)
        {
            Name = GetName();
            Help = "np (<user>)\t\t- Returns your currently playing music (from Last.fm). You can also supply a specific username to check.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(np|nowplaying)$", RegexOptions.IgnoreCase))
            {
                string lastfmName = "";

                if (message.ParameterList.Count > 0)
                {
                    lastfmName = message.ParameterList[0];
                }
                else
                {
                    if (AccountMap.ContainsKey(message.User.Name.ToUpper()))
                    {
                        lastfmName = AccountMap[message.User.Name.ToUpper()];
                    }
                    else
                    {
                        lastfmName = message.User.Name;
                    }
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

    class NowPlayingRegister : Function
    {
        public NowPlayingRegister(MoronBot moronBot)
        {
            Name = GetName();
            Help = "npregister/nplink <Last.fm Name>\t\t- Links the specified Last.fm account name to your IRC name.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(np(register|link))$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    if (NowPlaying.AccountMap.ContainsKey(message.User.Name.ToUpper()))
                    {
                        NowPlaying.AccountMap[message.User.Name.ToUpper()] = message.ParameterList[0];
                    }
                    else
                    {
                        NowPlaying.AccountMap.Add(message.User.Name.ToUpper(), message.ParameterList[0]);
                    }
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Last.fm account \"" + message.ParameterList[0] + "\" is now linked to IRC name \"" + message.User.Name + "\""));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't specify a Last.fm account name! Format is \"" + message.Command + " <Last.fm Account>\"", message.ReplyTo));
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
