using System;
using System.Collections.Generic;

using CwIRC;
using MBFunctionInterface;

namespace MoronBot.Functions
{
    public class CTCP : Function
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
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "VERSION MoronBot version... <1.0 I guess, running on" + GetOSString() + Convert.ToChar((byte)1), message.ReplyTo) };
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

        string GetOSString()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return " Windows";
                case PlatformID.Unix:
                    return " Unix";
                case PlatformID.MacOSX:
                    return " Mac OS X";
                case PlatformID.Xbox:
                    return " an... XBox?! What?!?";
                default:
                    return "... actually I have no idea what this is running on.";
            }
        }
    }
}
