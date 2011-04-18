using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Bot
{
    class Do : Function
    {
        public Do(MoronBot moronBot)
        {
            Name = GetName();
            Help = "do <text>\t\t- 'Does' the given text in the current channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(do)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Do, message.Parameters, message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Do what?", message.ReplyTo));
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
