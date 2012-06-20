using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    /// <summary>
    /// A Function which returns the date of a specified event
    /// </summary>
    /// <suggester>Dragoon</suggester>
    public class DateOf : Function
    {
        public DateOf()
        {
            Help = "dateof <event> - returns the date of the specified event.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^dateof$", RegexOptions.IgnoreCase))
                return null;

            if (message.ParameterList.Count == 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Date of what?", message.ReplyTo) };

            List<Events.EventStruct> reversedList = null;
            Events.EventStruct eventStruct = new Events.EventStruct();

            lock (Events.eventListLock)
            {
                reversedList = new List<Events.EventStruct>(Events.EventList);

                eventStruct = Events.EventList.Find(s =>
                    s.EventDate > DateTime.UtcNow &&
                    Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
            }

            reversedList.Reverse();

            if (eventStruct.EventName == null)
                eventStruct = reversedList.Find(s =>
                    s.EventDate <= DateTime.UtcNow &&
                    Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));

            if (eventStruct.EventName == null)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in the events list.", message.ReplyTo) };

            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "The date for \"" + eventStruct.EventName + "\" is " + eventStruct.EventDate.ToString(@"yyyy-MM-dd \a\t HH:mm (UTC)"), message.ReplyTo) };
        }
    }
}