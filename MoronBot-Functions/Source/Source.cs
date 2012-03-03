using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace GitHub
{
    public class Source : Function
    {
        public Source()
        {
            Help = "source (<function>) - Returns a link to the specified function's source on MoronBot's GitHub site. If no function is specified, then it links to the homepage instead.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(source)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "https://github.com/MatthewCox/MoronBot/", message.ReplyTo);
                return;
            }

            string command = null;//moronBot.CommandList.Find(s => s.IndexOf(message.ParameterList[0], StringComparison.InvariantCultureIgnoreCase) >= 0);
            if (command != null)
            {
                FuncInterface.SendResponse(ResponseType.Say, "https://github.com/MatthewCox/MoronBot/tree/master/MoronBot-Functions/" + command + "/", message.ReplyTo);
                return;
            }
            else
            {
                FuncInterface.SendResponse(ResponseType.Say, "Functions directory: https://github.com/MatthewCox/MoronBot/tree/master/MoronBot-Functions", message.ReplyTo);
                return;
            }
        }
    }
}
