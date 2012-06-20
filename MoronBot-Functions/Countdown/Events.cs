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
    public class Events : Function
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
                        // Both null
                        return 0;
                    else
                        // B is greater
                        return -1;
                }
                else
                {
                    if (b.EventDate == null)
                        // A is greater
                        return 1;
                    else
                        return a.EventDate.CompareTo(b.EventDate);
                }
            }
        }

        public static List<EventStruct> EventList = new List<EventStruct>();

        public static Object eventListLock = new Object();
        static Object eventFileLock = new Object();

        public Events()
        {
            Help = "events/upcoming (<days>) - Tells you all of the events coming up in the next week, or the next <days>, if you give a number parameter.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadEvents();
        }

        ~Events()
        {
            SaveEvents();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(events|upcoming)$", RegexOptions.IgnoreCase))
                return null;

            double daysAhead = 7;
            if (message.ParameterList.Count > 0)
            {
                if (!Double.TryParse(message.ParameterList[0], out daysAhead))
                    daysAhead = 7;
                if (Math.Abs(daysAhead) > 36500)
                    daysAhead = 36500;
                daysAhead = Math.Abs(daysAhead);
            }

            List<string> weekEvents = new List<string>();

            lock (eventListLock)
            {
                foreach (EventStruct weekEvent in EventList)
                {
                    if (weekEvent.EventDate > DateTime.UtcNow &&
                        weekEvent.EventDate < DateTime.UtcNow.AddDays(daysAhead))
                    {
                        weekEvents.Add(weekEvent.EventName);
                    }
                }
            }

            if (weekEvents.Count == 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "There are no events in the coming " + daysAhead + " days!", message.ReplyTo) };

            List<IRCResponse> responses = new List<IRCResponse>();
            responses.Add(new IRCResponse(ResponseType.Say, "Events in the next " + daysAhead + " days:", message.ReplyTo));

            string events = weekEvents[0];
            foreach (string weekEvent in weekEvents)
            {
                if ((events + " | " + weekEvent).Length < 400)
                {
                    events += " | " + weekEvent;
                }
                else
                {
                    responses.Add(new IRCResponse(ResponseType.Say, events, message.ReplyTo));
                    events = weekEvent;
                }
            }
            responses.Add(new IRCResponse(ResponseType.Say, events, message.ReplyTo));

            return responses;
        }

        public static void SaveEvents()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}Events.xml", Path.DirectorySeparatorChar));

            FileUtils.CreateDirIfNotExists(fileName);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;

            lock (eventFileLock) lock (eventListLock)
            {
                using (XmlWriter writer = XmlWriter.Create(fileName, xws))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Events");

                    foreach (EventStruct eventStruct in EventList)
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
        }

        static void LoadEvents()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}Events.xml", Path.DirectorySeparatorChar));
            if (!File.Exists(fileName))
                return;

            XmlDocument doc = new XmlDocument();

            lock (eventFileLock) lock (eventListLock)
            {
                using (FileStream fstream = File.OpenRead(fileName))
                {
                    using (StreamReader reader = new StreamReader(fstream))
                    {
                        doc.Load(reader);
                        XmlNode root = doc.DocumentElement;

                        foreach (XmlNode eventNode in root.SelectNodes(@"/Events/Event"))
                        {
                            EventStruct eventStruct = new EventStruct();
                            eventStruct.EventName = eventNode.SelectSingleNode("EventName").FirstChild.Value;
                            eventStruct.EventDate = DateTime.Parse(eventNode.SelectSingleNode("EventDate").FirstChild.Value, CultureInfo.InvariantCulture.DateTimeFormat);

                            EventList.Add(eventStruct);
                        }
                    }
                }
            }
        }
    }
}
