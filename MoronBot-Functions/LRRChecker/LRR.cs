using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    public class LRR : Function
    {
        public LRR()
        {
            Help = "lrr (<series>) - returns a link to the latest LRR video, or the latest of a series if you specify one";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^l(r|l)r$", RegexOptions.IgnoreCase))
                return null;

            List<string> feeds = new List<string>(LRRChecker.FeedMap.Keys);

            if (message.Parameters.Trim().Length > 0)
            {
                string feed = ReplaceAliases(message.Parameters);

                feed = feeds.Find(s =>
                    s.ToLowerInvariant() == feed.ToLowerInvariant());
                if (feed == String.Empty)
                    return new List<IRCResponse>() { new IRCResponse(
                        ResponseType.Say,
                        "\"" + message.Parameters + "\" is not one of the LRR series being monitored.",
                        message.ReplyTo) };

                return new List<IRCResponse>() { new IRCResponse(
                    ResponseType.Say,
                    "Latest " + feed + ": " + LRRChecker.FeedMap[feed].LatestTitle + " (" + LRRChecker.FeedMap[feed].LatestLink + ")",
                    message.ReplyTo) };
            }
            else
            {
                string newest = String.Empty;
                DateTime newestDate = DateTime.MinValue;
                foreach (string feed in feeds)
                {
                    if (LRRChecker.FeedMap[feed].LastUpdate > newestDate)
                    {
                        newestDate = LRRChecker.FeedMap[feed].LastUpdate;
                        newest = feed;
                    }
                }

                return new List<IRCResponse>() { new IRCResponse(
                    ResponseType.Say,
                    "Latest " + newest + ": " + LRRChecker.FeedMap[newest].LatestTitle + " (" + LRRChecker.FeedMap[newest].LatestLink + ")",
                    message.ReplyTo) };
            }
        }

        string ReplaceAliases(string series)
        {
            Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
            aliases.Add("Unskippable", new List<string>() {
                "unskipable", "unskip", "us", "u" });
            aliases.Add("LRR", new List<string>() {
                "loadingreadyrun", "l", "llr" });
            aliases.Add("Feed Dump", new List<string>() {
                "feed", "dump", "fdump", "fd", "f" });
            aliases.Add("CheckPoint", new List<string>() {
                "check", "point", "cp", "c" });
            aliases.Add("LRRCast", new List<string>() {
                "podcast", "cast", "lrrc", "llrc", "lcast", "lc" });

            foreach (var alias in aliases)
            {
                if (alias.Value.Contains(series.ToLowerInvariant()))
                {
                    return alias.Key;
                }
            }

            return series;
        }
    }
}
