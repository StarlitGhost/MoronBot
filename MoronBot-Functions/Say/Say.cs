using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Bot
{
    public class Say : Function
    {
        public Say()
        {
            Help = "say <text> - Says the given text in the current channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(say)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Say what?", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponse(ResponseType.Say, message.Parameters, message.ReplyTo);
            return;
        }
    }
}
