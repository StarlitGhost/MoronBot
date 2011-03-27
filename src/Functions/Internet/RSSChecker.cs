using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using Bitly;

namespace MoronBot.Functions.Internet
{
    class RSSChecker : Function
    {
        public struct Feed
        {
            string URL;
            DateTime LastUpdate;
        }

        public static Dictionary<string, string> FeedMap = new Dictionary<string, string>();

        public RSSChecker(MoronBot moronBot)
        {
            Name = GetName();
            Help = "Automatic function, scans RSS feeds and reports new items in the channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            //LoadFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        ~RSSChecker()
        {
            //SaveFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            string feedURL = "http://www.mspaintadventures.com/rss/rss.xml";

            XmlDocument doc = new XmlDocument();

            HttpWebRequest request = WebRequest.Create(feedURL) as HttpWebRequest;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                doc.Load(reader);

            }
        }

        //public void SaveFeeds(string fileName)
        //{
        //    XmlWriterSettings xws = new XmlWriterSettings();
        //    xws.Indent = true;
        //    xws.NewLineOnAttributes = true;
        //    using (XmlWriter writer = XmlWriter.Create(fileName, xws))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("Links");

        //        foreach (KeyValuePair<string, string> link in AccountMap)
        //        {
        //            writer.WriteStartElement("Link");

        //            writer.WriteElementString("IRCName", link.Key);
        //            writer.WriteElementString("LastFMName", link.Value);

        //            writer.WriteEndElement();
        //        }

        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }
        //}

        //public void LoadFeeds(string fileName)
        //{
        //    if (!File.Exists(fileName))
        //        return;

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(new StreamReader(File.OpenRead(fileName)));
        //    XmlNode root = doc.DocumentElement;

        //    foreach (XmlNode linkNode in root.SelectNodes(@"/Links/Link"))
        //    {
        //        string IRCName = linkNode.SelectSingleNode("IRCName").FirstChild.Value;
        //        string LastFMName = linkNode.SelectSingleNode("LastFMName").FirstChild.Value;

        //        AccountMap.Add(IRCName, LastFMName);
        //    }
        //}
    }
}
