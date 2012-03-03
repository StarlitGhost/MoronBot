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

            FuncInterface.PRIVMSGReceived += privmsgReceived;
        }

        void privmsgReceived(object sender, BotMessage message)
        {
            if (!message.CTCP)
                return;

            switch (message.CTCPString)
            {
                case "PING":
                    FuncInterface.SendResponse(ResponseType.Notice, message.MessageString, message.ReplyTo);
                    return;
                case "VERSION":
                    FuncInterface.SendResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "VERSION MoronBot version... <1.0 I guess, running on" + GetOSString() + Convert.ToChar((byte)1), message.ReplyTo);
                    return;
                case "TIME":
                    FuncInterface.SendResponse(ResponseType.Notice, Convert.ToChar((byte)1) + "TIME " + DateTime.Now.ToString("hh:mm tt, ddd dd MMM yyyy") + Convert.ToChar((byte)1), message.ReplyTo);
                    return;
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
