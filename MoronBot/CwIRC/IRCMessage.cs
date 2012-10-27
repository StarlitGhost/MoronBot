using System;
using System.Collections.Generic;
using System.Linq;

namespace CwIRC
{
    public class IRCMessage
    {
        string messageType;
        public string Type
        {
            get { return messageType; }
        }

        public struct UserStruct
        {
            public string Name;
            public string User;
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

        bool ctcp = false;
        public bool CTCP
        {
            get { return ctcp; }
        }

        string ctcpString;
        public string CTCPString
        {
            get { return ctcpString; }
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
                    string[] userHostArray = userArray[1].Split('@');
                    User.User = userHostArray[0];
                    User.Hostmask = userHostArray[1];
                }
                else
                {
                    User.Name = messageList[0].TrimStart(':');
                    User.User = "";
                    User.Hostmask = "";
                }

                if (messageType == "JOIN")
                {
                    TargetType = TargetTypes.CHANNEL;
                    replyTo = messageList[2].TrimStart(':');
                }
                else if (messageType == "PRIVMSG" || messageType == "TOPIC")
                {
                    if (messageList[2].TrimStart(':').StartsWith("#"))
                    {
                        TargetType = TargetTypes.CHANNEL;
                        replyTo = messageList[2].TrimStart(':').ToLowerInvariant();
                    }
                    else
                    {
                        TargetType = TargetTypes.USER;
                        replyTo = User.Name.ToLowerInvariant();
                    }

                    messageString = String.Join(" ", messageList.ToArray(), 3, messageList.Count - 3);
                    if (messageString.StartsWith(":"))
                    {
                        messageString = messageString.Substring(1, messageString.Length - 1);
                    }

                    if (MessageString.StartsWith(Convert.ToChar((byte)1).ToString()))
                    {
                        int messageEnd = MessageString.IndexOf(Convert.ToChar((byte)1), 1);
                        if (messageEnd > 1)
                        {
                            ctcpString = MessageString.Substring(1, messageEnd - 1);
                            ctcp = true;
                        }
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
