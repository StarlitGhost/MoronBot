using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MBFunctionInterface
{
    public class BotMessage : CwIRC.IRCMessage
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

        public BotMessage(string message, string currentNick) : base(message)
        {
            parameters = "";
            parameterList = new List<string>();
            command = "";
            if (Type == "PRIVMSG")
            {
                Match match = Regex.Match(MessageString, "^(\\||" + currentNick + "(,|:)?[ ])", RegexOptions.IgnoreCase);
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
                    else
                    {
                        parameters = "";
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
