using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Utility
{
    public class Log : Function
    {
        public Log()
        {
            Help = "log - Posts the daily log to pastebin.com, and returns a link to it.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(log)$", RegexOptions.IgnoreCase))
            {
                DateTime date = DateTime.Now.IsDaylightSavingTime() ? DateTime.UtcNow.AddHours(1.0) : DateTime.UtcNow;

                if (message.ParameterList.Count > 0)
                {
                    int dayOffset;
                    if (Int32.TryParse(message.ParameterList[0], out dayOffset))
                    {
                        if (dayOffset < 0 && dayOffset > -99999)
                        {
                            date = date.AddDays(dayOffset);
                        }
                    }
                }

                string fileDate = date.ToString(@" yyyy-MM-dd");
                string filePath = string.Format(@".{0}logs{0}" + Settings.Instance.Server + fileDate + @"{0}" + message.ReplyTo + @".txt", Path.DirectorySeparatorChar);

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string logText = reader.ReadToEnd();

                        string logLink = URL.Pastebin(logText, message.ReplyTo + " Log" + fileDate, "10M", "text", "1");

                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Log for" + fileDate + " posted: " + logLink + " (link expires in 10 mins)", message.ReplyTo) };
                    }
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "I don't have a log file for" + fileDate + " :(", message.ReplyTo) };
                }
            }
            return null;
        }
    }
}
