
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
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
            List<IRCResponse> responses = new List<IRCResponse>();

            List<string> keysToDelete = new List<string>();

            foreach (KeyValuePair<string, List<Tell.TellMessage>> kvp in Tell.MessageMap)
            {
                if (Regex.IsMatch(message.User.Name, kvp.Key, RegexOptions.IgnoreCase))
                {
                    foreach (Tell.TellMessage msg in kvp.Value)
                    {
                        responses.Add(new IRCResponse(ResponseType.Say, msg.Message, message.ReplyTo));
                        responses.Add(new IRCResponse(ResponseType.Say, "^ from " + msg.From + " on " + msg.SentDate, message.ReplyTo));
                    }

                    keysToDelete.Add(kvp.Key);
                }
            }
            foreach (string key in keysToDelete)
            {
                Tell.MessageMap[key].Clear();
                Tell.MessageMap.Remove(key);
            }

            if (responses.Count > 0)
            {
                return responses;
            }
            else
            {
                return null;
            }
        }
    }
}