using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;
using MBUtilities.Channel;

namespace Internet
{
    public class NowPlaying : Function
    {
        public static Dictionary<string, string> AccountMap = new Dictionary<string, string>();

        public NowPlaying()
        {
            Help = "np (<user>) - Returns your currently playing music (from LastFM). You can also supply a specific username to check.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadLinks(Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}NowPlayingLinks.xml", Path.DirectorySeparatorChar)));
        }

        ~NowPlaying()
        {
            SaveLinks(Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}NowPlayingLinks.xml", Path.DirectorySeparatorChar)));
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(np|nowplaying)$", RegexOptions.IgnoreCase))
            {
                string lastfmName = "";

                if (message.ParameterList.Count > 0)
                {
                    if (AccountMap.ContainsKey(message.ParameterList[0].ToUpper()))
                    {
                        lastfmName = AccountMap[message.ParameterList[0].ToUpper()];
                    }
                    else
                    {
                        lastfmName = message.ParameterList[0];
                    }
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

                URL.WebPage recentFeed = new URL.WebPage();

                try
                {
                    recentFeed = URL.FetchURL("http://ws.audioscrobbler.com/1.0/user/" + lastfmName + "/recenttracks.rss");
                }
                catch (System.Net.WebException ex)
                {
                    string filePath = string.Format(@".{0}logs{0}errors.txt", Path.DirectorySeparatorChar);
                    Logger.Write(ex.ToString(), filePath);
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "User \"" + lastfmName + "\" not found on LastFM", message.ReplyTo) };
                }

                Match track = Regex.Match(recentFeed.Page, @"<item>\s*?<title>(?<band>.+?)–(?<song>.+?)</title>\s*?<link>(?<link>.+?)</link>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (track.Success)
                {
                    string band = track.Groups["band"].Value;
                    string song = track.Groups["song"].Value;

                    string songMessage = "\"" + song.Trim() + "\" by " + band.Trim();

                    songMessage += " (" + /*ChannelList.EvadeChannelLinkBlock(message, URL.Shorten(*/track.Groups["link"].Value/*))*/ + ")";

                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, songMessage, message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "User \"" + lastfmName + "\" exists on LastFM, but hasn't scrobbled any music to it", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }

        public void SaveLinks(string fileName)
        {
            FileUtils.CreateDirIfNotExists(fileName);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create(fileName, xws))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Links");

                foreach (KeyValuePair<string, string> link in AccountMap)
                {
                    writer.WriteStartElement("Link");

                    writer.WriteElementString("IRCName", link.Key);
                    writer.WriteElementString("LastFMName", link.Value);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void LoadLinks(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(new StreamReader(File.OpenRead(fileName)));
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode linkNode in root.SelectNodes(@"/Links/Link"))
            {
                string IRCName = linkNode.SelectSingleNode("IRCName").FirstChild.Value;
                string LastFMName = linkNode.SelectSingleNode("LastFMName").FirstChild.Value;

                AccountMap.Add(IRCName, LastFMName);
            }
        }
    }

    public class NowPlayingRegister : Function
    {
        public NowPlayingRegister()
        {
            Help = "npregister/nplink <LastFM Name> - Links the specified LastFM account name to your IRC name.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
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
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "LastFM account \"" + message.ParameterList[0] + "\" is now linked to IRC name \"" + message.User.Name + "\"", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't specify a LastFM account name! Format is \"" + message.Command + " <LastFM Account>\"", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
