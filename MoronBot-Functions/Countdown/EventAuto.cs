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
    public class EventAuto : Function
    {
        DateTime _lastDate;

        public EventAuto()
        {
            Help = "Automatic function that will output events as they occur.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            _lastDate = DateTime.UtcNow;
        }

        ~EventAuto()
        {
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if ((DateTime.UtcNow - _lastDate).Minutes < 1)
                return null;

            for (int i = 0; i < TimeTill.EventList.Count; i++)
            {
                TimeTill.EventStruct autoEvent = TimeTill.EventList[i];
                if (autoEvent.EventDate < DateTime.UtcNow && autoEvent.EventDate > _lastDate)
                {
                    List<IRCResponse> responses = new List<IRCResponse>();
                    TimeTill.EventStruct lastEvent = autoEvent;
                    while (autoEvent.EventDate < DateTime.UtcNow)
                    {
                        responses.Add(new IRCResponse(ResponseType.Say, "It's time for \"" + autoEvent.EventName + "\"!", message.ReplyTo));
                        lastEvent = autoEvent;
                        autoEvent = TimeTill.EventList[++i];
                    }
                    _lastDate = lastEvent.EventDate;

                    if (responses.Count > 1)
                        return responses;
                    return null;
                }
            }

            return null;
        }
    }
}
