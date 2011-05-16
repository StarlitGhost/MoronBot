using System.Text.RegularExpressions;

using CwIRC;
using System.Collections.Generic;

namespace MoronBot.Functions.Bot
{
    class Join : Function
    {
        public Join()
        {
            Help = "join <channel>\t\t- Joins the specified channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(join)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string output = "";
                    foreach (string parameter in message.ParameterList)
                    {
                        output += "JOIN ";
                        if (!parameter.StartsWith("#"))
                        {
                            output += "#";
                        }
                        output += parameter + "\r\n";
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, output, "") };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, message.User.Name + ", you didn't say where I should join.", message.ReplyTo) };
                }
            }
            return null;
        }
    }
}
