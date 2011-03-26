using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Bot
{
    class Leave : Function
    {
        public Leave(MoronBot moronBot)
        {
            Name = GetName();
            Help = "leave/gtfo [<channel>]\t- Leaves the current channel, or the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            AccessList.Add("Tyranic-Moron");
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(leave|gtfo)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + message.Parameters, ""));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + Settings.Instance.LeaveMessage, ""));
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
