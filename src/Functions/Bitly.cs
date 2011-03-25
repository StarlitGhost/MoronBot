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
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, @"^(bit\.?ly|shorten)$", RegexOptions.IgnoreCase))
            {
                // URL given
                if (message.ParameterList.Count > 0)
                {
                    string bitlyURL = Utilities.URL.Shorten(message.Parameters);
                    if (bitlyURL != null)
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, bitlyURL, message.ReplyTo));
                        return;
                    }
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No URL detected in your message.", message.ReplyTo));
                    return;
                }
                // No URL given
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't give a URL to shorten!", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
