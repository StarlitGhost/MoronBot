using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Say : Function
    {
        public Say(MoronBot moronBot)
        {
            Name = GetName();
            Help = "say <text>\t\t- Says the given text in the current channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(say)$", RegexOptions.IgnoreCase))
            {
                if (message.Parameters.Length > 0)
                {
                    return new IRCResponse(ResponseType.Say, message.Parameters, message.ReplyTo);
                }
                else
                {
                    return new IRCResponse(ResponseType.Say, "Say what?", message.ReplyTo);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
