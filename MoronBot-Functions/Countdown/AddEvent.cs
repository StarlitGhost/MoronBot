using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class AddEvent : Function
    {
        public AddEvent()
        {
            Help = "(add/set)event <date> <event>  - Adds an 'event' to the list of events used in Countdown. <date> is in dd-MM-yyyy format. You can put the date in brackets if you want to specify time and so forth.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^((add|set)?event)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 1)
                {
                    Countdown.EventStruct eventStruct = new Countdown.EventStruct();
                    bool parseSuccess = false;

                    if (message.ParameterList[0].StartsWith("("))
                    {
                        Match dateMessage = Regex.Match(message.Parameters, @"^\((.+)\) (.+)");
                        if (dateMessage.Success)
                        {
                            parseSuccess = DateTime.TryParse(dateMessage.Groups[1].Value, out eventStruct.EventDate);
                            eventStruct.EventDate = DateTime.SpecifyKind(eventStruct.EventDate, DateTimeKind.Utc);
                            eventStruct.EventName = dateMessage.Groups[2].Value;
                        }
                    }
                    else
                    {
                        parseSuccess = DateTime.TryParse(message.ParameterList[0], out eventStruct.EventDate);
                        eventStruct.EventDate = DateTime.SpecifyKind(eventStruct.EventDate, DateTimeKind.Utc);
                        eventStruct.EventName = message.Parameters.Remove(0, message.ParameterList[0].Length + 1);
                    }

                    if (parseSuccess)
                    {
                        if (Countdown.eventList.FindIndex(s => s.EventName == eventStruct.EventName) < 0)
                        {
                            Countdown.eventList.Add(eventStruct);
                            Countdown.eventList.Sort(Countdown.EventStruct.CompareEventStructsByDate);
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Added event \"" + eventStruct.EventName + "\" on " + eventStruct.EventDate.ToString(@"dd-MM-yyyy \a\t HH:mm"), message.ReplyTo) };
                        }
                        else
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Event \"" + eventStruct.EventName + "\" is already in the event list, on " + eventStruct.EventDate.ToString(@"dd-MM-yyyy \a\t HH:mm"), message.ReplyTo) };
                        }

                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Parsing of date: " + message.ParameterList[0] + " failed, expected format is dd-MM-yyyy", message.ReplyTo) };
                    }
                }
                else if (message.ParameterList.Count > 0)
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give an event!", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give a date and event!", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
