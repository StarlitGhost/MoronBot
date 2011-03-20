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
                if (message.Parameters.Length > 0)
                {
                    Match match = Regex.Match(message.Parameters, @"https?://[^\s]+", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string bitlyURL = API.Bit("tyranicmoron", "R_2cec505899bffdf2f88e0a15953661e6", match.Value, "Shorten");
                        return new IRCResponse(ResponseType.Say, bitlyURL, message.ReplyTo);
                    }
                    return new IRCResponse(ResponseType.Say, "No URL detected in your message.", message.ReplyTo);
                }
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
