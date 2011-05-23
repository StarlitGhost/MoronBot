using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Utility
{
    public class Time : Function
    {
        public Time()
        {
            Help = "time (<offset>)\t\t\t- Tells you the time... roughly. You can provide an offset from the bot's standard time (UTC), if you wish.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(time)$", RegexOptions.IgnoreCase))
            {
                DateTime date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "GMT Standard Time");

                if (message.ParameterList.Count > 0)
                {
                    double offset;
                    if (Double.TryParse(message.ParameterList[0], out offset))
                    {
                        if (Math.Abs(offset) <= 15)
                        {
                            date = date.AddHours(offset);
                        }
                        else
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Offset too large, valid offsets are between -15 and +15.", message.ReplyTo) };
                        }
                    }
                }

                int hour = date.Hour;
                string timeOfDayMessage = "morning";
                if (hour > 12 && hour < 17)
                {
                    timeOfDayMessage = "afternoon";
                }
                else if (hour >= 17)
                {
                    timeOfDayMessage = "evening";
                }

                int minuteSecond = (date.Minute * 100) + date.Second;
                string roughMinute;
                string timeMessage;
                if (hour == 23 && minuteSecond >= 5000 || hour == 0 && minuteSecond <= 1000)
                {
                    timeMessage = "midnight";
                }
                else if (hour >= 11 && minuteSecond >= 5000 && hour < 1 && minuteSecond <= 1000)
                {
                    timeMessage = "midday";
                }
                else
                {
                    if (minuteSecond >= 230 && minuteSecond < 730)
                    {
                        roughMinute = "5 past";
                    }
                    else if (minuteSecond >= 730 && minuteSecond < 1230)
                    {
                        roughMinute = "10 past";
                    }
                    else if (minuteSecond >= 1230 && minuteSecond < 1730)
                    {
                        roughMinute = "quarter past";
                    }
                    else if (minuteSecond >= 1730 && minuteSecond < 2230)
                    {
                        roughMinute = "20 past";
                    }
                    else if (minuteSecond >= 2230 && minuteSecond < 2730)
                    {
                        roughMinute = "25 past";
                    }
                    else if (minuteSecond >= 2730 && minuteSecond < 3230)
                    {
                        roughMinute = "half past";
                    }
                    else if (minuteSecond >= 3230 && minuteSecond < 3730)
                    {
                        hour++;
                        roughMinute = "25 to";
                    }
                    else if (minuteSecond >= 3730 && minuteSecond < 4230)
                    {
                        hour++;
                        roughMinute = "20 to";
                    }
                    else if (minuteSecond >= 4230 && minuteSecond < 4730)
                    {
                        hour++;
                        roughMinute = "quarter to";
                    }
                    else if (minuteSecond >= 4730 && minuteSecond < 5230)
                    {
                        hour++;
                        roughMinute = "10 to";
                    }
                    else if (minuteSecond >= 5230 && minuteSecond < 5730)
                    {
                        hour++;
                        roughMinute = "5 to";
                    }
                    else
                    {
                        if (minuteSecond >= 5730)
                        {
                            hour++;
                        }
                        roughMinute = "O'Clock";
                    }

                    if (hour >= 13)
                    {
                        hour -= 12;
                    }
                    if (hour == 0)
                    {
                        hour = 12;
                    }

                    if (roughMinute == "O'Clock")
                    {
                        timeMessage = hour + " " + roughMinute + " in the " + timeOfDayMessage;
                    }
                    else
                    {
                        timeMessage = roughMinute + " " + hour + " in the " + timeOfDayMessage;
                    }
                }

                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "It's " + timeMessage + ".", message.ReplyTo) };
            }
            else
            {
                return null;
            }
        }
    }
}
