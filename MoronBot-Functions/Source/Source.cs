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
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(source)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string command = null;//moronBot.CommandList.Find(s => s.IndexOf(message.ParameterList[0], StringComparison.InvariantCultureIgnoreCase) >= 0);
                    if (command != null)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "https://github.com/Tyranic-Moron/MoronBot/tree/master/MoronBot-Functions/" + command + "/", message.ReplyTo) };
                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, /*"Function \"" + message.ParameterList[0] + "\" not found, linking to */"Functions directory" +/* instead*/": https://github.com/Tyranic-Moron/MoronBot/tree/master/MoronBot-Functions", message.ReplyTo) };
                    }
                }
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "https://github.com/Tyranic-Moron/MoronBot/", message.ReplyTo) };
            }
            else
            {
                return null;
            }
        }
    }
}
