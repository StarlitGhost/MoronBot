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

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^dateof$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Date of what?", message.ReplyTo);
                return;
            }

            List<TimeTill.EventStruct> reversedList = new List<TimeTill.EventStruct>(TimeTill.EventList);
            reversedList.Reverse();

            TimeTill.EventStruct eventStruct = TimeTill.EventList.Find(s =>
                s.EventDate > DateTime.UtcNow &&
                Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));

            if (eventStruct.EventName == null)
                eventStruct = reversedList.Find(s =>
                    s.EventDate <= DateTime.UtcNow &&
                    Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));

            if (eventStruct.EventName == null)
            {
                FuncInterface.SendResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in the events list.", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponse(ResponseType.Say, "The date for \"" + eventStruct.EventName + "\" is " + eventStruct.EventDate.ToString(@"yyyy-MM-dd \a\t HH:mm (UTC)"), message.ReplyTo);
            return;
        }
    }
}