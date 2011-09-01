using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace GitHub
{
    public class LastCommit : Function
    {
        public LastCommit()
        {
            Help = "(last)commit/(last)change <url> - Tells you what the last changes to MoronBot were.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^((last)?(commit|change))$", RegexOptions.IgnoreCase))
            {
                URL.WebPage commitFeed = URL.FetchURL("https://github.com/MatthewCox/MoronBot/commits/master.atom");

                if (commitFeed.Page != null)
                {
                    MatchCollection commitMessages = Regex.Matches(commitFeed.Page, @"width:81ex'>(.+)?\&lt;/pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Last Commit Message: " + commitMessages[0].Groups[1].Value.Replace("\n", " | "), message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Couldn't fetch the commit feed, GitHub may be down?", message.ReplyTo) };
                }
            }

            return null;
        }
    }
}
