using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Internet
{
    public class Bitly : Function
    {
        public Bitly()
        {
            Help = "bitly/shorten <url> - Gives you a shortened version of a url, via bit.ly";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, @"^(bit\.?ly|shorten)$", RegexOptions.IgnoreCase))
            {
                // URL given
                if (message.ParameterList.Count > 0)
                {
                    string bitlyURL = URL.Shorten(message.Parameters);
                    if (bitlyURL != null)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, bitlyURL, message.ReplyTo) };
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
