using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace MBUtilities.Channel
{
    public static class ChannelList
    {
        static readonly object channelSync = new object();
        static readonly object userSync = new object();

        public static BindingList<Channel> Channels { get; set; }

        #region Events
        public static event VoidEventHandler ChannelListModified;
        public static void OnChannelListModified()
        {
            if (ChannelListModified != null)
                ChannelListModified(new object());
        }

        public static event VoidEventHandler UserListModified;
        public static void OnUserListModified()
        {
            if (UserListModified != null)
                UserListModified(new object());
        }
        #endregion Events

        static ChannelList()
        {
            Channels = new BindingList<Channel>();
        }

        public static int GetChannelID(string channelName)
        {
            channelName = channelName.ToLowerInvariant();

            int channelID;

            lock (channelSync)
            {
                channelID = Channels.FindIndex(c => c.Name == channelName);

                if (channelID == -1)
                {
                    Channel channel = new Channel();
                    channel.Name = channelName;
                    Channels.Add(channel);
                    channelID = Channels.IndexOf(channel);

                    OnChannelListModified();
                }
            }

            return channelID;
        }

        static int GetUserID(string nick, int channelID)
        {
            nick = nick.ToLowerInvariant();

            int userID;

            lock (channelSync)
            {
                userID = Channels[channelID].Users.FindIndex(u => u.Nick == nick);

                if (userID == -1)
                {
                    User user = new User();
                    user.Nick = nick;
                    Channels[channelID].Users.Add(user);
                    userID = Channels[channelID].Users.IndexOf(user);

                    OnUserListModified();
                }
            }

            return userID;
        }

        public static int GetUserID(string nick, string channel, out int channelID)
        {
            channelID = GetChannelID(channel);
            return GetUserID(nick, channelID);
        }

        static List<char> GetChannelModes(string channel)
        {
            int channelID = GetChannelID(channel);
            return Channels[channelID].Modes;
        }

        public static bool ChannelHasMode(string channel, char mode)
        {
            List<char> modes = GetChannelModes(channel);

            return modes.Contains(mode);
        }

        static string EvadeLinkBlock(string url)
        {
            return url.Replace("//", "// ").Replace(".", " .");
        }

        public static string EvadeChannelLinkBlock(BotMessage message, string link)
        {
            return (message.TargetType == IRCMessage.TargetTypes.CHANNEL && ChannelList.ChannelHasMode(message.ReplyTo, 'U')) ? EvadeLinkBlock(link) : link;
        }

        public static string GetUserModes(string nick, string channel)
        {
            int channelID, userID = ChannelList.GetUserID(nick, channel, out channelID);

            return ChannelList.Channels[channelID].Users[userID].Symbols;
        }

        public static bool UserIsAnyOp(string nick, string channel)
        {
            string userModes = GetUserModes(nick, channel);
            if (userModes.Contains("~") || userModes.Contains("&") || userModes.Contains("@") || userModes.Contains("%"))
                return true;

            return false;
        }

        public static bool UserIsFounder(string nick, string channel)
        {
            if (GetUserModes(nick, channel).Contains("~"))
                return true;

            return false;
        }

        public static bool UserIsSop(string nick, string channel)
        {
            if (GetUserModes(nick, channel).Contains("&"))
                return true;

            return false;
        }

        public static bool UserIsOp(string nick, string channel)
        {
            if (GetUserModes(nick, channel).Contains("@"))
                return true;

            return false;
        }

        public static bool UserIsHop(string nick, string channel)
        {
            if (GetUserModes(nick, channel).Contains("%"))
                return true;

            return false;
        }

        public static bool UserIsVoiced(string nick, string channel)
        {
            if (GetUserModes(nick, channel).Contains("+"))
                return true;

            return false;
        }

        public static void ParseJOIN(BotMessage message)
        {
            int channelID, userID = GetUserID(message.User.Name, message.MessageList[2].TrimStart(':'), out channelID);

            lock (channelSync)
                Channels[channelID].Users[userID].Hostmask = message.User.Hostmask;

            OnUserListModified();
        }

        public static void ParsePART(BotMessage message, bool parterIsMe)
        {
            int channelID, userID = GetUserID(message.User.Name, message.MessageList[2].TrimStart(':'), out channelID);

            lock (channelSync)
                Channels[channelID].Users.RemoveAt(userID);

            OnUserListModified();
            if (parterIsMe)
            {
                lock (channelSync)
                    Channels.RemoveAt(channelID);

                OnChannelListModified();
            }
        }

        public static List<string> ParseQUIT(BotMessage message)
        {
            List<string> quittedChannels = new List<string>();

            lock (channelSync)
            {
                foreach (Channel c in Channels)
                {
                    int userID = c.Users.FindIndex(u => u.Nick == message.User.Name);
                    if (userID != -1)
                    {
                        c.Users.RemoveAt(userID);
                        quittedChannels.Add(c.Name);
                    }
                }
            }
            OnUserListModified();

            return quittedChannels;
        }

        public static void ParseMODE(BotMessage message)
        {
            string modeString = message.MessageList[3];
            if (message.MessageList.Count > 4) // User mode change
            {
                int channelID, userID = GetUserID(message.MessageList[4], message.MessageList[2], out channelID);

                bool addModes = modeString.StartsWith("+");
                modeString = modeString.TrimStart('+', '-');

                if (modeString.Contains("b")) return; // Ban message, ignore

                if (addModes)
                {
                    lock (channelSync)
                    {
                        foreach (char mode in modeString)
                        {
                            if (!Channels[channelID].Users[userID].Modes.Contains(mode))
                            {
                                Channels[channelID].Users[userID].Modes.Add(mode);
                            }

                            if (mode == 'q') Channels[channelID].Users[userID].Symbols += "~";
                            if (mode == 'o') Channels[channelID].Users[userID].Symbols += "@";
                            if (mode == 'h') Channels[channelID].Users[userID].Symbols += "%";
                            if (mode == 'v') Channels[channelID].Users[userID].Symbols += "+";
                        }
                    }
                }
                else
                {
                    lock (channelSync)
                    {
                        foreach (char mode in modeString)
                        {
                            if (Channels[channelID].Users[userID].Modes.Contains(mode))
                            {
                                Channels[channelID].Users[userID].Modes.Remove(mode);
                            }

                            if (mode == 'q') Channels[channelID].Users[userID].Symbols.Replace("~", "");
                            if (mode == 'o') Channels[channelID].Users[userID].Symbols.Replace("@", "");
                            if (mode == 'h') Channels[channelID].Users[userID].Symbols.Replace("%", "");
                            if (mode == 'v') Channels[channelID].Users[userID].Symbols.Replace("+", "");
                        }
                    }
                }
                OnUserListModified();
            }
            else // Channel mode change
            {
                if (message.MessageList[2].ToLowerInvariant() == message.User.Name)
                    return;

                int channelID = GetChannelID(message.MessageList[2]);

                ParseChannelModeString(modeString, channelID);

                OnChannelListModified();
            }
        }

        static void ParseChannelModeString(string modeString, int channelID)
        {
            bool addModes = modeString.StartsWith("+");
            modeString = modeString.TrimStart('+', '-');

            if (modeString == "") return;

            if (addModes)
            {
                lock (channelSync)
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
            else
            {
                lock (channelSync)
                {
                    foreach (char mode in modeString)
                    {
                        if (Channels[channelID].Modes.Contains(mode))
                        {
                            Channels[channelID].Modes.Remove(mode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Channel modes
        /// </summary>
        /// <param name="message"></param>
        public static void Parse324(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            ParseChannelModeString(message.MessageList[4], channelID);

            OnChannelListModified();
        }

        /// <summary>
        /// Channel topic
        /// </summary>
        /// <param name="message"></param>
        public static void Parse332(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            lock (channelSync)
                Channels[channelID].Topic = message.RawMessage.Substring(message.RawMessage.IndexOf(':', 1) + 1);

            OnChannelListModified();
        }

        /// <summary>
        /// WHO reply
        /// </summary>
        /// <param name="message"></param>
        public static void Parse352(BotMessage message)
        {
            int channelID = GetChannelID(message.MessageList[3]);

            int userID = GetUserID(message.MessageList[7], channelID);

            lock (channelSync)
                Channels[channelID].Users[userID].Hostmask = message.MessageList[5];
            //Channels[channelID].Users[userID].Symbols = Regex.Replace(message.MessageList[8], @"[a-zA-Z]+", "");
            
            OnUserListModified();
        }

        /// <summary>
        /// NAMES reply
        /// </summary>
        /// <param name="message"></param>
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
                    nick = match.Groups[2].Value.ToLowerInvariant();
                }
                else
                {
                    nick = match.Groups[1].Value.ToLowerInvariant();
                }

                lock (channelSync)
                {
                    if (Channels[channelID].Users.FindIndex(u => u.Nick == nick) == -1)
                    {
                        User user = new User();
                        user.Nick = nick;
                        user.Symbols = symbols;

                        Channels[channelID].Users.Add(user);
                    }
                }
            }
            OnUserListModified();
        }
    }
}
