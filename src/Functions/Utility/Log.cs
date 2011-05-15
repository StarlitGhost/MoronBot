using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MoronBot.Utilities;

namespace MoronBot.Functions.Utility
{
    class Log : Function
    {
        public Log(MoronBot moronBot)
        {
            Name = GetName();
            Help = "log\t\t- Posts the daily log to pastebin.com, and returns a link to it.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(log)$", RegexOptions.IgnoreCase))
            {
                DateTime date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "GMT Standard Time");

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
                string filePath = @".\logs\" + Settings.Instance.Server + fileDate + @"\" + message.ReplyTo + @".txt";

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string logText = reader.ReadToEnd();

                        string logLink = URL.Pastebin(logText, message.ReplyTo + " Log" + fileDate);

                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Log for" + fileDate + " posted: " + logLink + " (link expires in 10 mins)", message.ReplyTo));
                    }
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "I don't have a log file for" + fileDate + " :(", message.ReplyTo));
                }
            }
        }
    }
}
