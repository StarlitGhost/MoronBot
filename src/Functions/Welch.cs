using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Welch : Function
    {
        Random rand = new Random();
        List<string> welchList = new List<string>();

        public Welch(MoronBot moronBot)
        {
            Name = GetName();
            Help = "welch (<number>)\t\t- Returns a random \"Thing Mr. Welch can no longer do in an RPG\", or a specific one if you add a number.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            // Copied from http://theglen.livejournal.com/16735.html, had to find-replace fancy quotes with normal ones.
            StreamReader welchFile = new StreamReader(File.OpenRead("welch.txt"));

            while (!welchFile.EndOfStream)
            {
                welchList.Add(welchFile.ReadLine());
            }

            welchFile.Close();
        }
        
        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(welch)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    int number = 0;
                    if (Int32.TryParse(message.ParameterList[0], out number))
                    {
                        number -= 1;
                        if (number >= 0 && number < welchList.Count)
                        {
                            return new IRCResponse(ResponseType.Say, welchList[number], message.ReplyTo);
                        }
                    }
                    return new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + welchList.Count, message.ReplyTo);
                }
                else
                {
                    return new IRCResponse(ResponseType.Say, welchList[rand.Next(welchList.Count)], message.ReplyTo);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
