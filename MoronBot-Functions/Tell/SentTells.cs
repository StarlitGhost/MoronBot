using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    /// <summary>
    /// A Function which returns all of the messages that the user has sent, that have yet to be received.
    /// </summary>
    /// <suggester>Aeltrius</suggester>
    public class SentTells : Function
    {
        public SentTells()
        {
            Help = "s(ent)tells - Tells you all of the messages you have sent that have yet to be received.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^((outstanding|s(ent)?)tells)$", RegexOptions.IgnoreCase))
                return;

            List<string> responses = new List<string>();

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
                            responses.Add("To " + kvp.Key);
                        }
                        responses.Add(" > ");
                    }
                }
            }

            if (responses.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "There are no messages from you that have not already been received.", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponses(ResponseType.Say, responses, message.ReplyTo);
            return;
        }
    }
}