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
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(say)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, message.Parameters, message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Say what?", message.ReplyTo));
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
