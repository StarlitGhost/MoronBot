using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;

namespace MoronBot.Functions.Utility
{
    class Countdown : Function
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

        public Countdown(MoronBot moronBot)
        {
            Name = GetName();
            Help = "countdown/time(un)till (<event>)\t\t- Tells you the amount of time left until the specified event. Without a parameter, it will tell you how long until the next desertbus.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadEvents(Settings.Instance.Server + ".events.xml");
        }

        ~Countdown()
        {
            SaveEvents(Settings.Instance.Server + ".events.xml");
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
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
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo));
                        return;
                    }
                }
                else
                {
                    eventStruct = eventList.Find(s => s.EventName == "Desert Bus 5");
                }
                timeSpan = eventStruct.EventDate - DateTime.Now;
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, eventStruct.EventName + " will occur in " + timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s)", message.ReplyTo));
                return;
            }
            else
            {
                return;
            }
        }

        public void SaveEvents(string fileName)
        {
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
                    writer.WriteElementString("EventDate", eventStruct.EventDate.ToString());

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
                eventStruct.EventDate = DateTime.Parse(eventNode.SelectSingleNode("EventDate").FirstChild.Value);

                Countdown.eventList.Add(eventStruct);
            }
        }
    }

    class AddEvent : Function
    {
        public AddEvent(MoronBot moronBot)
        {
            Name = GetName();
            Help = "(add)event <date> <event> \t\t- Adds an 'event' to the list of events used in Countdown. <date> is in dd-MM-yyyy format. You can put the date in brackets if you want to specify time and so forth.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^((add)?event)$", RegexOptions.IgnoreCase))
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
                            eventStruct.EventName = dateMessage.Groups[2].Value;
                        }
                    }
                    else
                    {
                        parseSuccess = DateTime.TryParse(message.ParameterList[0], out eventStruct.EventDate);
                        eventStruct.EventName = message.Parameters.Remove(0, message.ParameterList[0].Length + 1);
                    }
                    
                    if (parseSuccess)
                    {
                        if (Countdown.eventList.FindIndex(s => s.EventName == eventStruct.EventName) < 0)
                        {
                            Countdown.eventList.Add(eventStruct);
                            Countdown.eventList.Sort(Countdown.EventStruct.CompareEventStructsByDate);
                            moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Added event \"" + eventStruct.EventName + "\" on " + eventStruct.EventDate.ToString(@"dd-MM-yyyy \a\t HH:mm"), message.ReplyTo));
                            return;
                        }
                        else
                        {
                            moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Event \"" + eventStruct.EventName + "\" is already in the event list, on " + eventStruct.EventDate.ToString(@"dd-MM-yyyy \a\t HH:mm"), message.ReplyTo));
                            return;
                        }

                    }
                    else
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Parsing of date: " + message.ParameterList[0] + " failed, expected format is dd-MM-yyyy", message.ReplyTo));
                        return;
                    }
                }
                else if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't give an event!", message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't give a date and event!", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    class RemoveEvent : Function
    {
        public RemoveEvent(MoronBot moronBot)
        {
            Name = GetName();
            Help = "r(emove)event <event> \t\t- Removes the specified 'event' from the list of events used in Countdown.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
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
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No event matching \"" + message.Parameters + "\" found in event list!", message.ReplyTo));
                        return;
                    }
                    else
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Event \"" + Countdown.eventList[index].EventName + "\", with date \"" + Countdown.eventList[index].EventDate.ToString() + "\" removed from the event list!", message.ReplyTo));
                        Countdown.eventList.RemoveAt(index);
                        return;
                    }
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't specify an event to remove!", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    class Upcoming : Function
    {
        public Upcoming(MoronBot moronBot)
        {
            Name = GetName();
            Help = "upcoming (<days>)\t\t- Tells you all of the events coming up in the next week, or the next <days>, if you give a number parameter.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(upcoming)$", RegexOptions.IgnoreCase))
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
                    if (weekEvent.EventDate > DateTime.Now && weekEvent.EventDate < DateTime.Now.AddDays(daysAhead))
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

                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Events in the next " + daysAhead + " days:", message.ReplyTo));
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, events, message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "There are no events in the coming " + daysAhead + " days!", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
