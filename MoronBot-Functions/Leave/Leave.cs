using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Bot
{
    public class Leave : Function
    {
        public Leave()
        {
            Help = "leave/gtfo [<channel>] - Leaves the current channel, or the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(leave|gtfo)$", RegexOptions.IgnoreCase))
            {
                if (message.TargetType == IRCMessage.TargetTypes.CHANNEL && ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
                {
                    if (message.ParameterList.Count > 0)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + message.Parameters, "") };
                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + Settings.Instance.LeaveMessage, "") };
                    }
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You need to be an operator to get rid of me :P", message.ReplyTo) };
                }
            }
            return null;
        }
    }
}
