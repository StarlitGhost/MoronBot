using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MBFunctionInterface;

namespace MBUtilities.Channel
{
    public static class ChannelList
    {
        public static BindingList<Channel> Channels { get; set; }

        static int GetChannelID(string channelName)
        {
            if (Channels == null) Channels = new BindingList<Channel>();

            int channelID = Channels.FindIndex(c => c.Name == channelName);

            if (channelID == -1)
            {
                Channel channel = new Channel();
                channel.Name = channelName;
                Channels.Add(channel);
                channelID = Channels.IndexOf(channel);
            }

            return channelID;
        }

        static int GetUserID(string nick, int channelID)
        {
            int userID = Channels[channelID].Users.FindIndex(u => u.Nick == nick);

            if (userID == -1)
            {
                User user = new User();
                user.Nick = nick;
                Channels[channelID].Users.Add(user);
                userID = Channels[channelID].Users.IndexOf(user);
            }

            return userID;
        }

        static int GetUserID(string nick, string channel, out int channelID)
        {
            channelID = GetChannelID(channel);
            return GetUserID(nick, channelID);
        }

        public static void ParseJOIN(BotMessage message)
        {
            int channelID, userID = GetUserID(message.User.Name, message.MessageList[2].TrimStart(':'), out channelID);

            Channels[channelID].Users[userID].Hostmask = message.User.Hostmask;
        }

        public static void ParsePART(BotMessage message)
        {
            int channelID, userID = GetUserID(message.User.Name, message.MessageList[2].TrimStart(':'), out channelID);

            Channels[channelID].Users.RemoveAt(userID);
        }

        public static void ParseQUIT(BotMessage message)
        {
            foreach (Channel c in Channels)
            {
                int userID = c.Users.FindIndex(u => u.Nick == message.User.Name);
                if (userID != -1) c.Users.RemoveAt(userID);
            }
        }

        public static void Parse324(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            string modeString = message.MessageList[4].TrimStart('+');

            if (modeString != "")
            {
                foreach (char mode in modeString)
                {
                    if (!Channels[channelID].Modes.Contains(mode))
                    {
                        Channels[channelID].Modes.Add(mode);
                    }
                }
            }
        }

        public static void Parse332(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            Channels[channelID].Topic = message.RawMessage.Substring(message.RawMessage.IndexOf(':', 1) + 1);
        }

        public static void Parse352(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            int userID = GetUserID(message.MessageList[7], channelID);

            Channels[channelID].Users[userID].Hostmask = message.MessageList[5];
            Channels[channelID].Users[userID].Symbols = message.MessageList[8].TrimStart('H', 'G');
        }

        public static void Parse353(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[4]);

            string nameString = message.RawMessage.Substring(message.RawMessage.LastIndexOf(':') + 1);
            List<string> names = nameString.Split(' ').ToList();

            foreach (string name in names)
            {
                if (name == "") continue;

                string nick = "", symbols = "";

                Match match = Regex.Match(name, @"([~@%&+]+)?(.+)");
                if (match.Groups.Count > 2)
                {
                    symbols = match.Groups[1].Value;
                    nick = match.Groups[2].Value;
                }
                else
                {
                    nick = match.Groups[1].Value;
                }

                if (Channels[channelID].Users.FindIndex(u => u.Nick == nick) == -1)
                {
                    User user = new User();
                    user.Nick = nick;
                    user.Symbols = symbols;

                    Channels[channelID].Users.Add(user);
                }
            }
        }
    }
}
