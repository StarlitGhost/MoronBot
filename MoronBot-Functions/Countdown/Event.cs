using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class Event : Function
    {
        public Event()
        {
            Help = "event <date> <event> - Adds an event to the list of events used by TimeTill and TimeSince. <date> is in dd-MM-yyyy format. Put the date in () if you want to specify time and your offset from UTC, eg: (25-12-2012 9:00 -5)";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^event$", RegexOptions.IgnoreCase))
                return null;

            if (message.ParameterList.Count <= 1)
            {
                if (message.ParameterList.Count > 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give an event!", message.ReplyTo) };
                else
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give a date and event!", message.ReplyTo) };
            }

            TimeTill.EventStruct eventStruct = new TimeTill.EventStruct();
            bool parseSuccess = false;

            if (message.ParameterList[0].StartsWith("("))
            {
                Match dateMessage = Regex.Match(message.Parameters, @"^\((.+)\) (.+)");
                if (dateMessage.Success)
                {
                    DateTimeOffset dateTimeOffset;
                    parseSuccess = DateTimeOffset.TryParse(dateMessage.Groups[1].Value, new CultureInfo("en-GB"), DateTimeStyles.AssumeUniversal, out dateTimeOffset);

                    eventStruct.EventDate = dateTimeOffset.UtcDateTime;

                    eventStruct.EventName = dateMessage.Groups[2].Value;
                }
            }
            else
            {
                DateTimeOffset dateTimeOffset;
                parseSuccess = DateTimeOffset.TryParse(message.ParameterList[0], new CultureInfo("en-GB"), DateTimeStyles.AssumeUniversal, out dateTimeOffset);

                eventStruct.EventDate = dateTimeOffset.UtcDateTime;

                eventStruct.EventName = message.Parameters.Remove(0, message.ParameterList[0].Length + 1);
            }

            if (!parseSuccess)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Parsing of date: " + message.ParameterList[0] + " failed, expected format is dd-MM-yyyy", message.ReplyTo) };

            eventStruct.EventName = StringUtils.StripIRCFormatChars(eventStruct.EventName);

            if (TimeTill.EventList.FindIndex(s => s.EventName == eventStruct.EventName) >= 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Event \"" + eventStruct.EventName + "\" is already in the event list, on " + eventStruct.EventDate.ToString(@"yyyy-MM-dd \a\t HH:mm (UTC)"), message.ReplyTo) };

            TimeTill.EventList.Add(eventStruct);
            TimeTill.EventList.Sort(TimeTill.EventStruct.CompareEventStructsByDate);
            TimeTill.SaveEvents();
            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Added event \"" + eventStruct.EventName + "\" on " + eventStruct.EventDate.ToString(@"yyyy-MM-dd \a\t HH:mm (UTC)"), message.ReplyTo) };
        }
    }
}
