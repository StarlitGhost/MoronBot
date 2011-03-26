using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Bot
{
    class Nick : Function
    {
        public Nick(MoronBot moronBot)
        {
            Name = GetName();
            Help = "nick <nick>\t\t- Changes the bot's nick to the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;

            AccessList.Add("Tyranic-Moron");
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(nick(name)?|name)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Raw, "NICK " + message.ParameterList[0], ""));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Change my nick to what?", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
