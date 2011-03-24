using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Countdown : Function
    {
        public struct Event
        {
            public string EventName;
            public DateTime EventDate;
        }

        public static List<Event> eventList = new List<Event>();

        public Countdown(MoronBot moronBot)
        {
            Name = GetName();
            Help = "countdown/time(un)till (<event>)\t\t- Tells you the amount of time left until the specified event. Without a parameter, it will tell you how long until the next desertbus.";
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
