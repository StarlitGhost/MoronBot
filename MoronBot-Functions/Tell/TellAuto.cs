
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    /// <summary>
    /// A Function which returns messages sent to a user, when that user says something in the bot's presence.
    /// </summary>
    public class TellAuto : Function
    {
        public TellAuto()
        {
            Help = "Automatic function that will output messages sent to a particular user when that user speaks.";
            Type = Types.UserList;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (message.Type != "PRIVMSG")
                return null;

            List<IRCResponse> responses = new List<IRCResponse>();

            List<string> keysToDelete = new List<string>();
            Dictionary<string, List<Tell.TellMessage>> keysToKeep = new Dictionary<string, List<Tell.TellMessage>>();

            lock (Tell.tellMapLock)
            {
                foreach (KeyValuePair<string, List<Tell.TellMessage>> kvp in Tell.MessageMap)
                {
                    if (Regex.IsMatch(message.User.Name, kvp.Key, RegexOptions.IgnoreCase))
                    {
                        foreach (Tell.TellMessage msg in kvp.Value)
                        {
                            if (msg.Target == "PM" || msg.Target == message.ReplyTo)
                            {
                                responses.Add(new IRCResponse(ResponseType.Say, message.User.Name + ": " + msg.Message, msg.Target == "PM" ? message.User.Name : msg.Target));
                                responses.Add(new IRCResponse(ResponseType.Say, "^ from " + msg.From + " on " + msg.SentDate, msg.Target == "PM" ? message.User.Name : msg.Target));
                            }
                            else
                            {
                                if (!keysToKeep.ContainsKey(kvp.Key))
                                    keysToKeep.Add(kvp.Key, new List<Tell.TellMessage>());
                                keysToKeep[kvp.Key].Add(msg);
                            }
                        }

                        keysToDelete.Add(kvp.Key);
                    }
                }
                foreach (string key in keysToDelete)
                {
                    Tell.MessageMap[key].Clear();
                    Tell.MessageMap.Remove(key);
                }
                Tell.MessageMap = Tell.MessageMap.Concat(keysToKeep).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            if (responses.Count == 0)
                return null;

            Tell.WriteMessages();

            return responses;
        }
    }
}