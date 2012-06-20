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
            if ((DateTime.UtcNow - _lastDate).Minutes < 5)
                return null;

            lock (Events.eventListLock)
            {
                for (int i = 0; i < Events.EventList.Count; i++)
                {
                    Events.EventStruct autoEvent = Events.EventList[i];

                    if (autoEvent.EventDate <= _lastDate || autoEvent.EventDate >= DateTime.UtcNow)
                        continue;

                    List<IRCResponse> responses = new List<IRCResponse>();
                    Events.EventStruct lastEvent = autoEvent;
                    while (autoEvent.EventDate < DateTime.UtcNow)
                    {
                        responses.Add(new IRCResponse(ResponseType.Say, "It's time for \"" + autoEvent.EventName + "\"!", message.ReplyTo));
                        lastEvent = autoEvent;
                        autoEvent = Events.EventList[++i];
                    }
                    _lastDate = lastEvent.EventDate;

                    if (responses.Count > 1)
                        return responses;

                    break;
                }
            }

            return null;
        }
    }
}
