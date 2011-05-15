﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;

namespace MoronBot.Functions.Utility
{
    class Tell : Function
    {
        public struct TellMessage
        {
            public string From;
            public string Message;
        }

        struct UserDateNumber
        {
            public string User { get; set; }
            public DateTime MessageTime { get; set; }
            public uint Number { get; set; }
        }

        public static Dictionary<string, List<TellMessage>> MessageMap = new Dictionary<string, List<TellMessage>>();

        List<UserDateNumber> userDateNumberList = new List<UserDateNumber>();

        public Tell(MoronBot moronBot)
        {
            Name = GetName();
            Help = "tell <user> <message>\t\t- Tells the specified user the specified message, when they next speak.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            ReadMessages(Settings.Instance.Server + ".tellMessages.xml");
        }

        ~Tell()
        {
            WriteMessages(Settings.Instance.Server + ".tellMessages.xml");
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(tell)$", RegexOptions.IgnoreCase))
            {
                int i = userDateNumberList.FindIndex(s => s.User == message.User.Name);

                if (i < 0)
                {
                    userDateNumberList.Add(new UserDateNumber() { User = message.User.Name, MessageTime = DateTime.Now, Number = 0 });
                    i = userDateNumberList.FindIndex(s => s.User == message.User.Name);
                }

                UserDateNumber userDateNumber = userDateNumberList[i];

                if (userDateNumber.MessageTime.AddMinutes(5).CompareTo(DateTime.Now) <= 0)
                {
                    userDateNumber.MessageTime = DateTime.Now;
                    userDateNumber.Number = 0;
                }
                else
                {
                    if (userDateNumber.Number < 3)
                    {
                        userDateNumber.MessageTime = DateTime.Now;
                        userDateNumber.Number++;
                        userDateNumberList[i] = userDateNumber;
                    }
                    else
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You've already sent 3 messages in the last 5 minutes, slow down!", message.ReplyTo));
                        return;
                    }
                }

                if (message.ParameterList.Count > 1)
                {
                    string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                    if (msg.Trim(' ').Length > 0)
                    {
                        if (!MessageMap.ContainsKey(message.ParameterList[0].ToUpper()))
                        {
                            MessageMap.Add(message.ParameterList[0].ToUpper(), new List<TellMessage>());
                        }
                        TellMessage tellMessage = new TellMessage();
                        tellMessage.From = "^ from " + message.User.Name + " on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (UTC zz)");
                        tellMessage.Message = msg;
                        MessageMap[message.ParameterList[0].ToUpper()].Add(tellMessage);
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Ok, I'll tell " + message.ParameterList[0] + " that when they next speak.", message.ReplyTo));
                        return;
                    }
                }

                if (message.ParameterList.Count > 0)
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't give a message for me to tell " + message.ParameterList[0], message.ReplyTo));
                    return;
                }
                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Tell who what?", message.ReplyTo));
                return;
            }
            return;
        }

        void WriteMessages(string fileName)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create(fileName, xws))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Users");

                foreach (KeyValuePair<string, List<TellMessage>> userMessages in MessageMap)
                {
                    writer.WriteStartElement("User");
                    writer.WriteElementString("Name", userMessages.Key);

                    writer.WriteStartElement("Messages");

                    foreach (TellMessage tellMessage in userMessages.Value)
                    {
                        writer.WriteStartElement("Message");

                        writer.WriteElementString("Text", tellMessage.Message);
                        writer.WriteElementString("From", tellMessage.From);
                        
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        void ReadMessages(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(new StreamReader(File.OpenRead(fileName)));
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode userNode in root.SelectNodes(@"/Users/User"))
            {
                string user = userNode.SelectSingleNode("Name").FirstChild.Value;

                List<TellMessage> tellMessages = new List<TellMessage>();

                foreach (XmlNode messageNode in userNode.SelectNodes(@"Messages/Message"))
                {
                    TellMessage tellMessage = new TellMessage();
                    tellMessage.Message = messageNode.SelectSingleNode("Text").FirstChild.Value;
                    tellMessage.From = messageNode.SelectSingleNode("From").FirstChild.Value;

                    tellMessages.Add(tellMessage);
                }

                MessageMap.Add(user, tellMessages);
            }
        }
    }

    class TellAuto : Function
    {
        public TellAuto(MoronBot moronBot)
        {
            Name = GetName();
            Help = "Automatic function that will output messages sent to a particular user when that user speaks.";
            Type = Types.UserList;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Tell.MessageMap.ContainsKey(message.User.Name.ToUpper()))
            {
                foreach (Tell.TellMessage msg in Tell.MessageMap[message.User.Name.ToUpper()])
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, msg.Message, message.ReplyTo));
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, msg.From, message.ReplyTo));
                }
                Tell.MessageMap[message.User.Name.ToUpper()].Clear();
                Tell.MessageMap.Remove(message.User.Name.ToUpper());
            }
            return;
        }
    }

}
