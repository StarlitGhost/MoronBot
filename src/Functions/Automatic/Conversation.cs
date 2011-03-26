using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Automatic
{
    class Conversation : Function
    {
        public Conversation(MoronBot moronBot)
        {
            Name = GetName(); ;
            Help = "A set of automatic functions that react to specific keywords/phrases in chat.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            // Cheese in message
            if (Regex.IsMatch(message.MessageString, "cheese", RegexOptions.IgnoreCase))
            {
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Do, "loves cheese", message.ReplyTo));
                return;
            }

            // Windmill in message
            if (Regex.IsMatch(message.MessageString, "windmill", RegexOptions.IgnoreCase))
            {
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "WINDMILLS DO NOT WORK THAT WAY!", message.ReplyTo));
                return;
            }

            // Someone has greeted MoronBot
            Match match = Regex.Match(message.MessageString, "^('?sup|hi|hey|hello|greetings|bonjour|salut|howdy|'?yo),?[ ]" + Settings.Instance.CurrentNick, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, match.Value.Split(' ')[0] + " " + message.User.Name, message.ReplyTo));
                return;
            }
            return;
        }
    }
}
