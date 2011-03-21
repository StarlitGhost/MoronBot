using System.Text.RegularExpressions;

using CwIRC;

using Bitly;

namespace MoronBot.Functions
{
    class Bitly : Function
    {
        public Bitly(MoronBot moronBot)
        {
            Name = GetName();
            Help = "bitly/shorten <url>\t\t- Gives you a shortened version of a url, via bit.ly";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(bit\\.?ly|shorten)$", RegexOptions.IgnoreCase))
            {
                // URL given
                if (message.Parameters.Length > 0)
                {
                    // Check that the 'URL' given, is actually a URL
                    Match match = Regex.Match(message.Parameters, @"https?://[^\s]+", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        // Use the Bitly API to shorten the given URL
                        string bitlyURL = API.Bit("tyranicmoron", "R_2cec505899bffdf2f88e0a15953661e6", match.Value, "Shorten");
                        return new IRCResponse(ResponseType.Say, bitlyURL, message.ReplyTo);
                    }
                    return new IRCResponse(ResponseType.Say, "No URL detected in your message.", message.ReplyTo);
                }
                // No URL given
                else
                {
                    return new IRCResponse(ResponseType.Say, "You didn't give a URL to shorten!", message.ReplyTo);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
