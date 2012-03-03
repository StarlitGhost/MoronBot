using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Bot
{
    public class Join : Function
    {
        public Join()
        {
            Help = "join <channel> - Joins the specified channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(join)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, message.User.Name + ", you didn't say where I should join.", message.ReplyTo);
                return;
            }

            string output = "";
            foreach (string parameter in message.ParameterList)
            {
                string channel = parameter;
                if (!channel.StartsWith("#"))
                {
                    channel = "#" + channel;
                }
                output += "JOIN " + channel + "\r\n";
            }

            FuncInterface.SendResponse(ResponseType.Raw, output, "");
            return;
        }
    }
}
