using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class TimeSince : Function
    {
        public TimeSince()
        {
            Help = "timesince/whenwas (<event>) - Tells you how long ago the specified event occurred. Without a parameter, it will tell you how long ago the last Desert Bus was.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(timesince|whenwas)$", RegexOptions.IgnoreCase))
            {
                TimeTill.EventStruct eventStruct;
                TimeSpan timeSpan;

                List<TimeTill.EventStruct> reversedList = new List<TimeTill.EventStruct>(TimeTill.EventList);
                reversedList.Reverse();

                if (message.ParameterList.Count > 0) // Parameters given
                {
                    // If an event in EventList is in the past, and the event name and command parameters match exactly,
                    // assign to eventStruct
                    eventStruct = reversedList.Find(s =>
                       s.EventDate <= DateTime.UtcNow &&
                       s.EventName == message.Parameters);

                    // If no matching message found
                    if (eventStruct.EventName == null)
                    {
                        // Same search as above, but using regex to match messages this time.
                        eventStruct = reversedList.Find(s =>
                            s.EventDate <= DateTime.UtcNow &&
                            Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                    }

                    // No matching events found
                    if (eventStruct.EventName == null)
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in the past events list!", message.ReplyTo) };
                }
                else // Parameters not given, assign last Desert Bus to eventStruct.
                {
                    eventStruct = reversedList.Find(s => s.EventName == "DB4 Ended");
                }

                timeSpan = DateTime.UtcNow - eventStruct.EventDate;
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, eventStruct.EventName + " occurred " + timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s) ago", message.ReplyTo) };
            }
            else
            {
                return null;
            }
        }
    }
}
