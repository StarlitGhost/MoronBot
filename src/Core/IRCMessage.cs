using System;
using System.Collections.Generic;
using System.Linq;

namespace CwIRC
{
    class IRCMessage
    {
        string messageType;
        public string Type
        {
            get { return messageType; }
        }

        public struct UserStruct
        {
            public string Name;
            public string Hostmask;
        }
        public UserStruct User;

        public enum TargetTypes
        {
            CHANNEL,
            USER
        }
        public TargetTypes TargetType;

        string replyTo;
        public string ReplyTo
        {
            get { return replyTo; }
        }

        List<string> messageList;
        public List<string> MessageList
        {
            get { return messageList; }
        }

        string messageString;
        public string MessageString
        {
            get { return messageString; }
        }

        string rawMessage;
        public string RawMessage
        {
            get { return rawMessage; }
        }

        public IRCMessage(string p_message)
        {
            rawMessage = p_message;

            if (rawMessage.StartsWith("PING "))
            {
                messageType = "PING";
                messageString = rawMessage.Split(' ')[1].TrimStart(':');
            }
            else
            {
                messageList = rawMessage.Split(' ').ToList<string>();

                messageType = messageList[1];

                if (messageList[0].Contains('!'))
                {
                    string[] userArray = messageList[0].Split('!');
                    User.Name = userArray[0].TrimStart(':');
                    User.Hostmask = userArray[1];
                }
                else
                {
                    User.Name = messageList[0];
                    User.Hostmask = "";
                }

                if (messageType == "PRIVMSG")
                {
                    if (messageList[2].TrimStart(':').StartsWith("#"))
                    {
                        TargetType = TargetTypes.CHANNEL;
                        replyTo = messageList[2].TrimStart(':');
                    }
                    else
                    {
                        TargetType = TargetTypes.USER;
                        replyTo = User.Name;
                    }

                    messageString = String.Join(" ", messageList.ToArray(), 3, messageList.Count - 3);
                    if (messageString.StartsWith(":"))
                    {
                        messageString = messageString.Substring(1, messageString.Length - 1);
                    }
                }
            }
        }

        public override string ToString()
        {
            return RawMessage;
        }
    }
}
