using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
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

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.MessageString, "cheese", RegexOptions.IgnoreCase))
            {
                return new IRCResponse(ResponseType.Do, "loves cheese", message.ReplyTo);
            }

            if (Regex.IsMatch(message.MessageString, "windmill", RegexOptions.IgnoreCase))
            {
                return new IRCResponse(ResponseType.Say, "WINDMILLS DO NOT WORK THAT WAY!", message.ReplyTo);
            }

            Match match = Regex.Match(message.MessageString, "^('?sup|hi|hey|hello|greetings|bonjour|salut|howdy|'?yo),?[ ]" + Settings.Instance.CurrentNick, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new IRCResponse(ResponseType.Say, match.Value.Split(' ')[0] + " " + message.User.Name, message.ReplyTo);
            }
            return null;
        }
    }
}
