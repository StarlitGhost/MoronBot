using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Bot
{
    public class Do : Function
    {
        public Do()
        {
            Help = "do <text> - 'Does' the given text in the current channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(do)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Do what?", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponse(ResponseType.Do, message.Parameters, message.ReplyTo);
            return;
        }
    }
}
