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
}
