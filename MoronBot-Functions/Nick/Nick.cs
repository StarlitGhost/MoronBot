using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities.Channel;

namespace Bot
{
    public class Nick : Function
    {
        public Nick()
        {
            Help = "nick <nick> - Changes the bot's nick to the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(nick(name)?|name)$", RegexOptions.IgnoreCase))
                return;

            if (message.TargetType != IRCMessage.TargetTypes.CHANNEL || !ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
            {
                FuncInterface.SendResponse(ResponseType.Say, "You need to be an operator to change my name :P", message.ReplyTo);
                return;
            }

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Change my nick to what?", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponse(ResponseType.Raw, "NICK " + message.ParameterList[0], "");
            return;
        }
    }
}
