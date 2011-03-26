using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Utility
{
    class Log : Function
    {
        Dictionary<string, List<string>> chatLog = new Dictionary<string, List<string>>();

        public Log(MoronBot moronBot)
        {
            Name = GetName();
            Help = "log\t\t- Sends you the last 10 things that were said in the channel you are in.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(log)$", RegexOptions.IgnoreCase))
            {
                Output(message, moronBot);
            }
            else
            {
                if (!chatLog.ContainsKey(message.ReplyTo))
                    chatLog[message.ReplyTo] = new List<string>();

                chatLog[message.ReplyTo].Add("<" + message.User.Name + "> " + message.MessageString);
                if (chatLog[message.ReplyTo].Count > 10)
                    chatLog[message.ReplyTo].RemoveAt(0);
            }
            return;
        }

        public void Output(BotMessage message, MoronBot moronBot)
        {
            List<IRCResponse> responseList = new List<IRCResponse>();

            if (!(chatLog[message.ReplyTo].Count > 0))
                return;

            foreach (string line in chatLog[message.ReplyTo])
            {
                responseList.Add(new IRCResponse(ResponseType.Notice, line, message.User.Name));
            }

            moronBot.MessageQueue.AddRange(responseList);
        }
    }
}
