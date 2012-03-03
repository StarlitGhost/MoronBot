using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Bot
{
    public class Ignore : Function
    {
        public Ignore()
        {
            Help = "ignore <user(s)> - Tells MoronBot to ignore the specified user(s). UserList functions will still work, however (TellAuto, for instance).";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(ignore)$", RegexOptions.IgnoreCase))
                return;

            if (message.TargetType != IRCMessage.TargetTypes.CHANNEL || !ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
            {
                FuncInterface.SendResponse(ResponseType.Say, "You need to be an operator to make me ignore people :P", message.ReplyTo);
                return;
            }

            foreach (string parameter in message.ParameterList)
            {
                if (!Settings.Instance.IgnoreList.Contains(parameter.ToUpper()))
                {
                    Settings.Instance.IgnoreList.Add(parameter.ToUpper());
                }
            }

            FuncInterface.SendResponse(ResponseType.Say, message.Parameters + (message.ParameterList.Count > 1 ? " are " : " is ") + "now ignored.", message.ReplyTo);
            return;
        }
    }

    public class Unignore : Function
    {
        public Unignore()
        {
            Help = "unignore <user(s)> - Tells MoronBot to unignore the specified user(s).";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(unignore)$", RegexOptions.IgnoreCase))
                return;

            if (message.TargetType != IRCMessage.TargetTypes.CHANNEL || !ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
            {
                FuncInterface.SendResponse(ResponseType.Say, "You need to be an operator to make me unignore people :P", message.ReplyTo);
                return;
            }

            foreach (string parameter in message.ParameterList)
            {
                if (Settings.Instance.IgnoreList.Contains(parameter.ToUpper()))
                {
                    Settings.Instance.IgnoreList.Remove(parameter.ToUpper());
                }
            }

            FuncInterface.SendResponse(ResponseType.Say, message.Parameters + (message.ParameterList.Count > 1 ? " are " : " is ") + "no longer ignored.", message.ReplyTo);
            return;
        }
    }
}
