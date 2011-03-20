using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Ignore : Function
    {
        public Ignore(MoronBot moronBot)
        {
            Name = GetName();
            Help = null;
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;

            AccessList.Add("Tyranic-Moron");
        }

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(ignore)$", RegexOptions.IgnoreCase))
            {
                foreach (string parameter in message.ParameterList)
                {
                    if (!Settings.Instance.IgnoreList.Contains(parameter))
                    {
                        Settings.Instance.IgnoreList.Add(parameter);
                    }
                }
                return new IRCResponse(ResponseType.Say, message.Parameters + " are now ignored.", message.ReplyTo);
            }
            else
            {
                return null;
            }
        }
    }
}
