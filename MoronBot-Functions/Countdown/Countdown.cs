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
    public class Countdown : Function
    {
        public struct EventStruct
        {
            public string EventName;
            public DateTime EventDate;

            public EventStruct(string name, DateTime date)
            {
                EventName = name;
                EventDate = date;
            }

            public static int CompareEventStructsByDate(EventStruct a, EventStruct b)
            {
                if (a.EventDate == null)
                {
                    if (b.EventDate == null)
                    {
                        // Both null
                        return 0;
                    }
                    else
                    {
                        // B is greater
                        return -1;
                    }
                }
                else
                {
                    if (b.EventDate == null)
                    {
                        // A is greater
                        return 1;
                    }
                    else
                    {
                        return a.EventDate.CompareTo(b.EventDate);
                    }
                }
            }
        }

        public static List<EventStruct> eventList = new List<EventStruct>();

        public Countdown()
        {
            Help = "countdown/time(un)till (<event>) - Tells you the amount of time left until the specified event. Without a parameter, it will tell you how long until the next desertbus.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadEvents(Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + "\\Events.xml"));
        }

        ~Countdown()
        {
            SaveEvents(Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + "\\Events.xml"));
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(countdown|time(un)?till)$", RegexOptions.IgnoreCase))
            {
                EventStruct eventStruct;
                TimeSpan timeSpan;
                if (message.ParameterList.Count > 0)
                {
                    eventStruct = eventList.Find(s => s.EventName == message.Parameters);
                    if (eventStruct.EventName == null)
                    {
                        eventStruct = eventList.Find(s => Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                    }
                    if (eventStruct.EventName == null)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo) };
                    }
                }
                else
                {
                    eventStruct = eventList.Find(s => s.EventName == "Desert Bus 5");
                }
                timeSpan = eventStruct.EventDate - DateTime.UtcNow;
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, eventStruct.EventName + " will occur in " + timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s)", message.ReplyTo) };
            }
            else
            {
                return null;
            }
        }

        public void SaveEvents(string fileName)
        {
            FileUtils.CreateDirIfNotExists(fileName);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;

            using (XmlWriter writer = XmlWriter.Create(fileName, xws))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Events");

                foreach (Countdown.EventStruct eventStruct in Countdown.eventList)
                {
                    writer.WriteStartElement("Event");

                    writer.WriteElementString("EventName", eventStruct.EventName);
                    writer.WriteElementString("EventDate", eventStruct.EventDate.ToString(CultureInfo.InvariantCulture.DateTimeFormat));

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void LoadEvents(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(new StreamReader(File.OpenRead(fileName)));
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode eventNode in root.SelectNodes(@"/Events/Event"))
            {
                EventStruct eventStruct = new EventStruct();
                eventStruct.EventName = eventNode.SelectSingleNode("EventName").FirstChild.Value;
                eventStruct.EventDate = DateTime.Parse(eventNode.SelectSingleNode("EventDate").FirstChild.Value, CultureInfo.InvariantCulture.DateTimeFormat);

                Countdown.eventList.Add(eventStruct);
            }
        }
    }

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
                    index = Countdown.eventList.FindIndex(s => s.EventName == message.Parameters);
                    if (index < 0)
                    {
                        index = Countdown.eventList.FindIndex(s => Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                    }
                    if (index < 0)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo) };
                    }
                    else
                    {
                        List<IRCResponse> response = new List<IRCResponse>();
                        response.Add(new IRCResponse(ResponseType.Say, "Event \"" + Countdown.eventList[index].EventName + "\", with date \"" + Countdown.eventList[index].EventDate.ToString() + "\" removed from the event list!", message.ReplyTo));
                        Countdown.eventList.RemoveAt(index);
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

    public class Upcoming : Function
    {
        public Upcoming()
        {
            Help = "upcoming/events (<days>) - Tells you all of the events coming up in the next week, or the next <days>, if you give a number parameter.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(upcoming|events)$", RegexOptions.IgnoreCase))
            {
                double daysAhead = 7;
                if (message.ParameterList.Count > 0)
                {
                    if (!Double.TryParse(message.ParameterList[0], out daysAhead))
                    {
                        daysAhead = 7;
                    }
                }
                List<string> weekEvents = new List<string>();
                foreach (Countdown.EventStruct weekEvent in Countdown.eventList)
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
