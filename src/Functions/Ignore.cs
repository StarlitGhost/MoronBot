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
            Help = "ignore <user(s)>\t\t- Tells MoronBot to ignore the specified user(s). UserList functions will still work, however (TellAuto, for instance).";
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

    class Unignore : Function
    {
        public Unignore(MoronBot moronBot)
        {
            Name = GetName();
            Help = "unignore <user(s)>\t\t- Tells MoronBot to unignore the specified user(s).";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;

            AccessList.Add("Tyranic-Moron");
        }

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(unignore)$", RegexOptions.IgnoreCase))
            {
                foreach (string parameter in message.ParameterList)
                {
                    if (!Settings.Instance.IgnoreList.Contains(parameter))
                    {
                        Settings.Instance.IgnoreList.Remove(parameter);
                    }
                }
                return new IRCResponse(ResponseType.Say, message.Parameters + " are no longer ignored.", message.ReplyTo);
            }
            else
            {
                return null;
            }
        }
    }
}
