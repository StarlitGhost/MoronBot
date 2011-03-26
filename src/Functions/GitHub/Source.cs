using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.GitHub
{
    class Source : Function
    {
        public Source(MoronBot moronBot)
        {
            Name = GetName();
            Help = "source (<function>)\t\t- Returns a link to the specified function's source on MoronBot's GitHub site. If no function is specified, then it links to the homepage instead.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(source)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string command = moronBot.CommandList.Find(s => s.IndexOf(message.ParameterList[0], StringComparison.InvariantCultureIgnoreCase) >= 0);
                    if (command != null)
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "https://github.com/Tyranic-Moron/MoronBot/tree/master/src/Functions/" + command + ".cs", message.ReplyTo));
                        return;
                    }
                    else
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Function \"" + message.ParameterList[0] + "\" not found, linking to Functions directory instead: https://github.com/Tyranic-Moron/MoronBot/tree/master/src/Functions", message.ReplyTo));
                        return;
                    }
                }
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "https://github.com/Tyranic-Moron/MoronBot/", message.ReplyTo));
                return;
            }
            else
            {
                return;
            }
        }
    }
}
