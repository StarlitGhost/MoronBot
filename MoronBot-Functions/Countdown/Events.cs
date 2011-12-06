using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{

    public class Events : Function
    {
        public Events()
        {
            Help = "events/upcoming (<days>) - Tells you all of the events coming up in the next week, or the next <days>, if you give a number parameter.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(events|upcoming)$", RegexOptions.IgnoreCase))
            {
                double daysAhead = 7;
                if (message.ParameterList.Count > 0)
                {
                    if (!Double.TryParse(message.ParameterList[0], out daysAhead))
                    {
                        daysAhead = 7;
                    }
                    if (Math.Abs(daysAhead) > 36500)
                    {
                        daysAhead = 36500;
                    }
                    daysAhead = Math.Abs(daysAhead);
                }
                List<string> weekEvents = new List<string>();
                foreach (TimeTill.EventStruct weekEvent in TimeTill.EventList)
                {
                    if (weekEvent.EventDate > DateTime.UtcNow && weekEvent.EventDate < DateTime.UtcNow.AddDays(daysAhead))
                    {
                        weekEvents.Add(weekEvent.EventName);
                    }
                }

                if (weekEvents.Count > 0)
                {
                    string events = "";
                    foreach (string weekEvent in weekEvents)
                    {
                        events += weekEvent + ", ";
                    }
                    events = events.Remove(events.Length - 2);

                    List<IRCResponse> responses = new List<IRCResponse>();
                    responses.Add(new IRCResponse(ResponseType.Say, "Events in the next " + daysAhead + " days:", message.ReplyTo));
                    responses.Add(new IRCResponse(ResponseType.Say, events, message.ReplyTo));
                    return responses;
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "There are no events in the coming " + daysAhead + " days!", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
