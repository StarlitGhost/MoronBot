using System;
using System.Collections.Generic;

using CwIRC;

namespace MoronBot.Functions
{
    class CTCP : Function
    {
        public CTCP()
        {
            Help = "Automatic function that returns certain CTCP queries (VERSION, PING, TIME)";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (message.CTCP)
            {
                if (message.CTCPString.StartsWith("PING"))
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, message.MessageString, message.ReplyTo) };
                }
                switch (message.CTCPString)
                {
                    case "VERSION":
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "VERSION MoronBot version... <1.0 I guess, running on a Windows machine someplace" + Convert.ToChar((byte)1), message.ReplyTo) };
                    case "TIME":
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "TIME " + DateTime.Now.ToString("hh:mm tt, ddd dd MMM yyyy") + Convert.ToChar((byte)1), message.ReplyTo) };
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
