using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class RemoveEvent : Function
    {
        public RemoveEvent()
        {
            Help = "r(emove)event <event> - Removes the specified event from the list of events used by TimeTill and TimeSince.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(r(emove)?event)$", RegexOptions.IgnoreCase))
                return null;

            if (message.ParameterList.Count == 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't specify an event to remove!", message.ReplyTo) };

            List<IRCResponse> response = new List<IRCResponse>();

            int index = 0;
            lock (Events.eventListLock)
            {
                index = Events.EventList.FindIndex(s => Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));

                if (index < 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo) };

                response.Add(new IRCResponse(ResponseType.Say, "Event \"" + Events.EventList[index].EventName + "\", with date \"" + Events.EventList[index].EventDate.ToString(@"yyyy-MM-dd \a\t HH:mm (UTC)") + "\" removed from the event list!", message.ReplyTo));
                Events.EventList.RemoveAt(index);
            }

            Events.SaveEvents();

            return response;
        }
    }
}
