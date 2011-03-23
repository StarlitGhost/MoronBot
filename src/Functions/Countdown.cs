using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Countdown : Function
    {
        public struct TellMessage
        {
            public string From;
            public string Message;
        }

        struct UserDateNumber
        {
            public string User { get; set; }
            public DateTime MessageTime { get; set; }
            public uint Number { get; set; }
        }

        public static Dictionary<string, List<TellMessage>> MessageMap = new Dictionary<string, List<TellMessage>>();

        List<UserDateNumber> userDateNumberList = new List<UserDateNumber>();

        public Countdown(MoronBot moronBot)
        {
            Name = GetName();
            Help = "countdown/time(un)till <event>\t\t- Tells you the amount of time left until the specified event.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(countdown|time(un)?till)$", RegexOptions.IgnoreCase))
            {
                if (message.Parameters.Length > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, message.Parameters, message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Say what?", message.ReplyTo));
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
