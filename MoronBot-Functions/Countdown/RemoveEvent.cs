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
            Help = "r(emove)event <event>  - Removes the specified 'event' from the list of events used in Countdown.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(r(emove)?event)$", RegexOptions.IgnoreCase))
            {
                int index = -1;

                if (message.ParameterList.Count > 0)
                {
                    index = TimeTill.EventList.FindIndex(s => s.EventName == message.Parameters);
                    if (index < 0)
                    {
                        index = TimeTill.EventList.FindIndex(s => Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                    }
                    if (index < 0)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo) };
                    }
                    else
                    {
                        List<IRCResponse> response = new List<IRCResponse>();
                        response.Add(new IRCResponse(ResponseType.Say, "Event \"" + TimeTill.EventList[index].EventName + "\", with date \"" + TimeTill.EventList[index].EventDate.ToString() + "\" removed from the event list!", message.ReplyTo));
                        TimeTill.EventList.RemoveAt(index);
                        TimeTill.SaveEvents();
                        return response;
                    }
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't specify an event to remove!", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
