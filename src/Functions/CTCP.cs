using System;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class CTCP : Function
    {
        public CTCP(MoronBot moronBot)
        {
            Name = GetName();
            Help = "Automatic function that returns certain CTCP queries (VERSION, PING, TIME)";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (message.CTCP)
            {
                if (message.CTCPString.StartsWith("PING"))
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, message.MessageString, message.ReplyTo));
                    return;
                }
                switch (message.CTCPString)
                {
                    case "VERSION":
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "VERSION MoronBot version... <1.0 I guess, running on a Windows machine someplace" + Convert.ToChar((byte)1), message.ReplyTo));
                        break;
                    case "TIME":
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "TIME " + DateTime.Now.ToString("hh:mm tt, ddd dd MMM yyyy") + Convert.ToChar((byte)1), message.ReplyTo));
                        break;
                }
                return;
            }
            else
            {
                return;
            }
        }
    }
}
