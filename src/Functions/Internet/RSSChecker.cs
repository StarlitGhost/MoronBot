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
            public string URL;
            public DateTime LastUpdate;

            public Feed(string url, DateTime lastUpdate)
            {
                URL = url;
                LastUpdate = lastUpdate;
            }
        }

        public static Dictionary<string, Feed> FeedMap = new Dictionary<string, Feed>();

        public RSSChecker(MoronBot moronBot)
        {
            Name = GetName();
            Help = "Automatic function, scans RSS feeds and reports new items in the channel.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            Feed homestuck = new Feed();
            homestuck.URL = "http://www.mspaintadventures.com/rss/rss.xml";
            homestuck.LastUpdate = DateTime.Now.AddDays(-2);
            FeedMap.Add("Homestuck", homestuck);

            //LoadFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        ~RSSChecker()
        {
            //SaveFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            //foreach (KeyValuePair<string, Feed> feed in FeedMap)
            //{


                Utilities.URL.WebPage feedPage = Utilities.URL.FetchURL(FeedMap["Homestuck"].URL);

                XmlDocument feedDoc = new XmlDocument();

                feedDoc.LoadXml(feedPage.Page);

                XmlNode firstItem = feedDoc.SelectSingleNode(@"/rss/channel/item");

                DateTime newestDate = new DateTime();
                DateTime.TryParse(firstItem.SelectSingleNode("pubDate").FirstChild.Value, out newestDate);

                if (newestDate > FeedMap["Homestuck"].LastUpdate)
                {
                    XmlNode oldestNew = feedDoc.SelectSingleNode(@"/rss/channel/item");

                    int numUpdates = 0;

                    foreach (XmlNode item in feedDoc.SelectNodes(@"/rss/channel/item"))
                    {
                        DateTime itemDate = new DateTime();
                        DateTime.TryParse(item.SelectSingleNode("pubDate").FirstChild.Value, out itemDate);

                        if (itemDate > FeedMap["Homestuck"].LastUpdate)
                        {
                            oldestNew = item;
                            numUpdates++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    FeedMap["Homestuck"] = new Feed(FeedMap["Homestuck"].URL, newestDate);

                    string itemTitle = oldestNew.SelectSingleNode("title").FirstChild.Value;
                    string itemLink = oldestNew.SelectSingleNode("link").FirstChild.Value;

                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Homestuck has updated, " + numUpdates + " new pages! New ones start here: " + itemTitle + " (" + itemLink + ")", message.ReplyTo));
                }
            //}
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
