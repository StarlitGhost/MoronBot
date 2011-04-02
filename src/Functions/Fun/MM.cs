using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Fun
{
    class MM : Function
    {
        Random rand = new Random();
        List<string> mmList = new List<string>();

        public MM(MoronBot moronBot)
        {
            Name = GetName();
            Help = "MM (<number>)\t\t- Returns a random \"Thing Players can no longer do in an MM game DM'd by Xela'\", or a specific one if you add a number.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            // Read mm.txt into mmList
            // I copied the class from Welch.cs
            StreamReader mmFile = new StreamReader(File.OpenRead("mm.txt"));

            while (!mmFile.EndOfStream)
            {
                mmList.Add(mmFile.ReadLine());
            }

            mmFile.Close();
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(mm)$", RegexOptions.IgnoreCase))
            {
                // Specific thing requested
                if (message.ParameterList.Count > 0)
                {
                    int number = 0;
                    if (Int32.TryParse(message.ParameterList[0], out number))
                    {
                        number -= 1;
                        if (number >= 0 && number < mmList.Count)
                        {
                            moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, mmList[number], message.ReplyTo));
                            return;
                        }
                    }
                    // Number too large or small, or not a number at all
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + mmList.Count, message.ReplyTo));
                    return;
                }
                // No specific thing requested
                else
                {
                    // Return a random thing
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, mmList[rand.Next(mmList.Count)], message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
