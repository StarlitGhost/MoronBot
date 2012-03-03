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

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(leave|gtfo)$", RegexOptions.IgnoreCase))
                return;

            if (message.TargetType != IRCMessage.TargetTypes.CHANNEL || !ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
            {
                FuncInterface.SendResponse(ResponseType.Say, "You need to be an operator to get rid of me :P", message.ReplyTo);
                return;
            }

            if (message.ParameterList.Count > 0)
            {
                FuncInterface.SendResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + message.Parameters, "");
                return;
            }
            else
            {
                FuncInterface.SendResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + Settings.Instance.LeaveMessage, "");
                return;
            }
        }
    }
}
