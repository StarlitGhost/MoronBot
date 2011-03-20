using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot
{
    class BotMessage : CwIRC.IRCMessage
    {
        string parameters;
        public string Parameters
        {
            get { return parameters; }
        }

        List<string> parameterList;
        public List<string> ParameterList
        {
            get { return parameterList; }
        }

        string command;
        public string Command
        {
            get { return command; }
        }

        public BotMessage(string message) : base(message)
        {
            parameters = "";
            parameterList = new List<string>();
            command = "";
            if (Type == "PRIVMSG")
            {
                Match match = Regex.Match(MessageString, "^(\\||" + Settings.Instance.CurrentNick + "(,|:)?[ ])", RegexOptions.IgnoreCase);
                parameters = MessageString.Substring(match.Value.Length);
                if (parameters.Length > 0)
                {
                    parameterList = parameters.Split(' ').ToList();
                    command = parameterList[0];
                    parameterList.RemoveAt(0);
                    if (parameterList.Count > 0)
                    {
                        parameters = parameters.Remove(0, command.Length + 1);
                    }
                }
                else
                {
                    parameters = "";
                }
            }
        }
    }
}
