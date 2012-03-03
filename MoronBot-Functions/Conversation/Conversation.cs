using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Automatic
{
    public class Conversation : Function
    {
        DateTime lastCheese = DateTime.Now.AddMinutes(-60);

        public Conversation()
        {
            Help = "A set of automatic functions that react to specific keywords/phrases in chat.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.PRIVMSGReceived += privmsgReceived;
        }

        void privmsgReceived(object sender, BotMessage message)
        {
            // Cheese in message
            if (Regex.IsMatch(message.MessageString, "cheese", RegexOptions.IgnoreCase))
            {
                if (lastCheese.AddMinutes(60).CompareTo(DateTime.Now) <= 0)
                {
                    lastCheese = DateTime.Now;
                    FuncInterface.SendResponse(ResponseType.Do, "loves cheese", message.ReplyTo);
                }
            }
        }
    }
}
