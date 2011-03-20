using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
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
        
        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(leave|gtfo)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    return new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + message.Parameters, "");
                }
                else
                {
                    return new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + Settings.Instance.LeaveMessage, "");
                }
            }
            else
            {
                return null;
            }
        }
    }
}
