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
    public class TimeTill : Function
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

        public TimeTill()
        {
            Help = "time(un)till/countdown (<event>) - Tells you the amount of time until the specified event. Without a parameter, it will tell you how long until the next Desert Bus.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadEvents();
        }

        ~TimeTill()
        {
            SaveEvents();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(time(un)?till|countdown)$", RegexOptions.IgnoreCase))
                return null;

            EventStruct eventStruct;
            TimeSpan timeSpan;

            if (message.ParameterList.Count > 0) // Parameters given
            {
                // If an event in EventList is in the future, and the event name and command parameters match exactly,
                // assign to eventStruct
                eventStruct = EventList.Find(s =>
                    s.EventDate > DateTime.UtcNow &&
                    s.EventName == message.Parameters);

                // If no matching message found
                if (eventStruct.EventName == null)
                {
                    // Same search as above, but using regex to match messages this time.
                    eventStruct = EventList.Find(s =>
                        s.EventDate > DateTime.UtcNow &&
                        Regex.IsMatch(s.EventName, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));
                }

                // No matching events found
                if (eventStruct.EventName == null)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in the future events list!", message.ReplyTo) };
            }
            else // Parameters not given, assign next Desert Bus to eventStruct.
            {
                eventStruct = EventList.Find(s =>
                    s.EventDate > DateTime.UtcNow &&
                    s.EventName.StartsWith("DB"));
            }

            timeSpan = eventStruct.EventDate - DateTime.UtcNow;
            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, eventStruct.EventName + " will occur in " + timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s)", message.ReplyTo) };
        }

        public static void SaveEvents()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}Events.xml", Path.DirectorySeparatorChar));
            
            FileUtils.CreateDirIfNotExists(fileName);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;

            using (XmlWriter writer = XmlWriter.Create(fileName, xws))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Events");

                foreach (TimeTill.EventStruct eventStruct in TimeTill.EventList)
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

        static void LoadEvents()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}Events.xml", Path.DirectorySeparatorChar));
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

                TimeTill.EventList.Add(eventStruct);
            }
        }
    }
}
