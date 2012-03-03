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
    /// <summary>
    /// A Function which returns the last song a user played, from Last.fm
    /// </summary>
    public class NowPlaying : Function
    {
        public static Dictionary<string, string> AccountMap = new Dictionary<string, string>();

        public NowPlaying()
        {
            Help = "np (<user>) - Returns your currently playing music (from LastFM). You can also supply a specific username to check.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;

            LoadLinks();
        }

        ~NowPlaying()
        {
            SaveLinks();
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(np|nowplaying)$", RegexOptions.IgnoreCase))
                return;

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
                Logger.Write(ex.ToString(), Settings.Instance.ErrorFile);
                FuncInterface.SendResponse(ResponseType.Say, "User \"" + lastfmName + "\" not found on LastFM", message.ReplyTo);
                return;
            }

            Match track = Regex.Match(recentFeed.Page, @"<item>\s*?<title>(?<band>.+?)–(?<song>.+?)</title>\s*?<link>(?<link>.+?)</link>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!track.Success)
            {
                FuncInterface.SendResponse(ResponseType.Say, "User \"" + lastfmName + "\" exists on LastFM, but hasn't scrobbled any music to it", message.ReplyTo);
                return;
            }

            string band = track.Groups["band"].Value;
            string song = track.Groups["song"].Value;

            string songMessage = "\"" + song.Trim() + "\" by " + band.Trim();

            songMessage += " (" + /*ChannelList.EvadeChannelLinkBlock(message, URL.Shorten(*/track.Groups["link"].Value/*))*/ + ")";

            FuncInterface.SendResponse(ResponseType.Say, songMessage, message.ReplyTo);
            return;
        }

        public static void SaveLinks()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}NowPlayingLinks.xml", Path.DirectorySeparatorChar));

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

        public void LoadLinks()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}NowPlayingLinks.xml", Path.DirectorySeparatorChar));

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

    public class NowPlayingLink : Function
    {
        public NowPlayingLink()
        {
            Help = "nplink <LastFM Name> - Links the specified LastFM account name to your IRC name.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^n(ow)?p(laying)?(link|reg(ister)?)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "You didn't specify a LastFM account name! Format is \"" + message.Command + " <LastFM Account>\"", message.ReplyTo);
                return;
            }

            string lastFMName = StringUtils.ReplaceNewlines(StringUtils.StripIRCFormatChars(message.ParameterList[0]), "");
            if (NowPlaying.AccountMap.ContainsKey(message.User.Name.ToUpper()))
            {
                NowPlaying.AccountMap[message.User.Name.ToUpper()] = lastFMName;
            }
            else
            {
                NowPlaying.AccountMap.Add(message.User.Name.ToUpper(), lastFMName);
            }
            NowPlaying.SaveLinks();
            FuncInterface.SendResponse(ResponseType.Say, "LastFM account \"" + lastFMName + "\" is now linked to IRC name \"" + message.User.Name + "\"", message.ReplyTo);
            return;
        }
    }
}
