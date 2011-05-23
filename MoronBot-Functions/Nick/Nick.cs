using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Bot
{
    public class Nick : Function
    {
        public Nick()
        {
            Help = "nick <nick>\t\t- Changes the bot's nick to the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;

            AccessList.Add("Tyranic-Moron");
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(nick(name)?|name)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "NICK " + message.ParameterList[0], "") };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Change my nick to what?", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
