using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Internet
{
    public class Googl : Function
    {
        public Googl()
        {
            Help = "googl/shorten <url> - Gives you a shortened version of a url, via Goo.gl";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, @"^(goo\.?gl|shorten)$", RegexOptions.IgnoreCase))
            {
                // URL given
                if (message.ParameterList.Count > 0)
                {
                    string shortURL = URL.Shorten(message.Parameters);
                    if (shortURL != null)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, shortURL, message.ReplyTo) };
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No URL detected in your message.", message.ReplyTo) };
                }
                // No URL given
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give a URL to shorten!", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
