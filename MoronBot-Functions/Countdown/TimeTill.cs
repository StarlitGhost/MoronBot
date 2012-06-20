using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class TimeTill : Function
    {
        public TimeTill()
        {
            Help = "time(un)till/countdown (<event>) - Tells you the amount of time until the specified event. Without a parameter, it will tell you how long until the next Desert Bus.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        ~TimeTill()
        {
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(time(un)?till|countdown)$", RegexOptions.IgnoreCase))
                return null;

            Events.EventStruct eventStruct;
            TimeSpan timeSpan;

            lock (Events.eventListLock)
            {
                if (message.ParameterList.Count > 0) // Parameters given
                {
                    // If an event in EventList is in the future, and the event name and command parameters match exactly,
                    // assign to eventStruct
                    eventStruct = Events.EventList.Find(s =>
                        s.EventDate > DateTime.UtcNow &&
                        s.EventName == message.Parameters);

                    // If no matching message found
                    if (eventStruct.EventName == null)
                    {
                        // Same search as above, but using regex to match messages this time.
                        eventStruct = Events.EventList.Find(s =>
                            s.EventDate > DateTime.UtcNow &&
                            Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                    }

                    // No matching events found
                    if (eventStruct.EventName == null)
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in the future events list!", message.ReplyTo) };
                }
                else // Parameters not given, assign next Desert Bus to eventStruct.
                {
                    eventStruct = Events.EventList.Find(s =>
                        s.EventDate > DateTime.UtcNow &&
                        s.EventName.StartsWith("DB"));
                }
            }

            timeSpan = eventStruct.EventDate - DateTime.UtcNow;
            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, eventStruct.EventName + " will occur in " + timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s)", message.ReplyTo) };
        }
    }
}
