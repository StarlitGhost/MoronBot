using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Fun
{
    public class Welch : Function
    {
        Random rand = new Random();
        List<string> welchList = new List<string>();

        public Welch()
        {
            Help = "welch (<number>) - Returns a random \"Thing Mr. Welch can no longer do in an RPG\", or a specific one if you add a number.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            // Read welch.txt into welchList
            // I copied the list directly from http://theglen.livejournal.com/16735.html, had to find-replace fancy quotes with normal ones.
            StreamReader welchFile = new StreamReader(File.OpenRead(Path.Combine(Settings.Instance.DataPath, "welch.txt")));

            while (!welchFile.EndOfStream)
            {
                welchList.Add(welchFile.ReadLine());
            }

            welchFile.Close();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(welch)$", RegexOptions.IgnoreCase))
            {
                // Specific thing requested
                if (message.ParameterList.Count > 0)
                {
                    int number = 0;
                    if (Int32.TryParse(message.ParameterList[0], out number))
                    {
                        number -= 1;
                        if (number >= 0 && number < welchList.Count)
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, welchList[number], message.ReplyTo) };
                        }
                    }
                    // Number too large or small, or not a number at all
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + welchList.Count, message.ReplyTo) };
                }
                // No specific thing requested
                else
                {
                    // Return a random thing
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, welchList[rand.Next(welchList.Count)], message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
