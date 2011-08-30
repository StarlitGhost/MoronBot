using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class SentTells : Function
    {
        public SentTells()
        {
            Help = "s(ent)tells - Tells you all of the messages you have sent that have yet to be received.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^((outstanding|s(ent)?)tells)$", RegexOptions.IgnoreCase))
            {
                List<IRCResponse> responses = new List<IRCResponse>();

                foreach (KeyValuePair<string, List<Tell.TellMessage>> kvp in Tell.MessageMap)
                {
                    bool fromUser = false;
                    foreach (Tell.TellMessage msg in kvp.Value)
                    {
                        if (msg.From.ToLowerInvariant() == message.User.Name.ToLowerInvariant())
                        {
                            if (!fromUser)
                            {
                                fromUser = true;
                                responses.Add(new IRCResponse(ResponseType.Say, "To " + kvp.Key, message.ReplyTo));
                            }
                            responses.Add(new IRCResponse(ResponseType.Say, " > " + msg.Message, message.ReplyTo));
                        }
                    }
                }

                if (responses.Count == 0)
                {
                    responses.Add(new IRCResponse(ResponseType.Say, "There are no messages from you that have not already been received.", message.ReplyTo));
                }
                
                return responses;
            }

            return null;
        }
    }
}