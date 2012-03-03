using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Utility
{
    /// <summary>
    /// A Function which posts the daily log for the current channel to pastebin.com, and returns a link to it.
    /// </summary>
    public class Log : Function
    {
        public Log()
        {
            Help = "log (-#) - Posts the daily log to pastebin.com, and returns a link to it. You can also fetch previous logs, by specifying an offset in days ('|log -1' would fetch yesterday's logs, for instance).";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^log$", RegexOptions.IgnoreCase))
                return;

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

            string fileDate = date.ToString(@"-yyyyMMdd");
            string filePath = string.Format(@".{0}logs{0}{1}{0}{2}{3}.txt", Path.DirectorySeparatorChar, Settings.Instance.Server, message.ReplyTo.ToLowerInvariant(), fileDate);

            if (!File.Exists(filePath))
            {
                FuncInterface.SendResponse(ResponseType.Say, "I don't have a log file for" + fileDate + " :(", message.ReplyTo);
                return;
            }

            string logText = Logger.ReadFileToString(filePath);
            string logLink = URL.Pastebin(logText, message.ReplyTo + " Log" + fileDate, "10M", "text", "1");
            FuncInterface.SendResponse(ResponseType.Say, string.Format("Log for {0} posted: {1} (link expires in 10 mins)", date.ToString(@"yyyy-MM-dd"), logLink), message.ReplyTo);
            return;
        }
    }
}
