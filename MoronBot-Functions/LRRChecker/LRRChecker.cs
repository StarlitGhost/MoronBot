using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Internet
{
    /// <summary>
    /// A Function which checks if LRR has released any new videos.
    /// </summary>
    public class LRRChecker : Function
    {
        public class Feed
        {
            public string URL;
            public DateTime LastUpdate;
            public DateTime LastCheck;
            public string LatestTitle;
            public string LatestLink;

            public Feed(string url, DateTime lastUpdate)
            {
                URL = url;
                LastUpdate = lastUpdate;
                LastCheck = DateTime.Now.AddMinutes(-10);
                LatestTitle = "";
                LatestLink = "";
            }
        }

        public static Dictionary<string, Feed> FeedMap = new Dictionary<string, Feed>();

        public LRRChecker()
        {
            Help = "Automatic function, scans LRR video RSS feeds and reports new items in the channel.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            FeedMap.Add("Unskippable",
                new Feed(
                    "http://www.escapistmagazine.com/rss/videos/list/82.xml",
                    DateTime.Now));

            FeedMap.Add("LRR",
                new Feed(
                    "http://www.escapistmagazine.com/rss/videos/list/123.xml",
                    DateTime.Now));

            FeedMap.Add("Feed Dump",
                new Feed(
                    "http://www.escapistmagazine.com/rss/videos/list/171.xml",
                    DateTime.Now));

            FeedMap.Add("CheckPoint",
                new Feed(
                    "http://penny-arcade.com/feed/show/checkpoint",
                    DateTime.Now));

            FeedMap.Add("LRRCast",
                new Feed(
                    "http://feeds.feedburner.com/lrrcast",
                    DateTime.Now));
        }

        ~LRRChecker()
        {
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            List<IRCResponse> responses = new List<IRCResponse>();

            List<string> feeds = new List<string>(FeedMap.Keys);
            foreach (string feed in feeds)
            {
                if (FeedMap[feed].LastCheck > DateTime.Now.AddMinutes(-10))
                    continue;

                FeedMap[feed].LastCheck = DateTime.Now;

                URL.WebPage feedPage;
                try
                {
                    feedPage = URL.FetchURL(FeedMap[feed].URL);
                }
                catch (System.Exception ex)
                {
                    Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                    continue;
                }

                XmlDocument feedDoc = new XmlDocument();

                feedDoc.LoadXml(feedPage.Page);

                XmlNode firstItem = feedDoc.SelectSingleNode(@"/rss/channel/item");

                DateTime newestDate = new DateTime();
                DateTime.TryParse(firstItem.SelectSingleNode("pubDate").FirstChild.Value, out newestDate);

                FeedMap[feed].LatestTitle = firstItem.SelectSingleNode("title").FirstChild.Value;
                FeedMap[feed].LatestLink = URL.Shorten(ChannelList.EvadeChannelLinkBlock(message, firstItem.SelectSingleNode("link").FirstChild.Value));

                if (newestDate > FeedMap[feed].LastUpdate)
                {
                    FeedMap[feed].LastUpdate = newestDate;

                    responses.Add(new IRCResponse(ResponseType.Say, "New " + feed + "! Title: " + FeedMap[feed].LatestTitle + " (" + FeedMap[feed].LatestLink + ")", message.ReplyTo));
                }
            }

            if (responses.Count == 0)
                return null;

            return responses;
        }
    }
}
