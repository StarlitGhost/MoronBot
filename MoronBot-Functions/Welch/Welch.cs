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
            Help = "welch (<number> / list) - Returns a random \"Thing Mr. Welch can no longer do in an RPG\", a specific one if you add a number, or posts the list to pastebin.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;

            // Read welch.txt into welchList
            // I copied the list directly from http://theglen.livejournal.com/16735.html, had to find-replace fancy quotes with normal ones.

            LoadList();
        }

        ~Welch()
        {
            SaveList();
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(welch)$", RegexOptions.IgnoreCase))
                return;

            if (welchList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Welch list failed to load, is welch.txt in the data directory?", message.ReplyTo);
                return;
            }

            if (message.ParameterList.Count == 0)
            {
                // Return a random thing
                FuncInterface.SendResponse(ResponseType.Say, welchList[rand.Next(welchList.Count)], message.ReplyTo);
                return;
            }

            // Specific thing requested
            if (message.ParameterList[0] == "list") // Post the list to pastebin, give link
            {
                string list = "";
                foreach (string item in welchList)
                {
                    list += item + "\n";
                }
                string url = URL.Pastebin(list, "Welch List", "10M", "text", "1");
                FuncInterface.SendResponse(ResponseType.Say, "Welch list posted: " + url + " (link expires in 10 mins)", message.ReplyTo);
                return;
            }
            else
            {
                int number = 0;
                if (Int32.TryParse(message.ParameterList[0], out number))
                {
                    number -= 1;
                    if (number >= 0 && number < welchList.Count)
                    {
                        FuncInterface.SendResponse(ResponseType.Say, welchList[number], message.ReplyTo);
                        return;
                    }
                }
                // Number too large or small, or not a number at all
                FuncInterface.SendResponse(ResponseType.Say, "Invalid number, range is 1-" + welchList.Count, message.ReplyTo);
                return;
            }
        }

        void LoadList()
        {
            welchList.AddRange(File.ReadAllLines(Path.Combine(Settings.Instance.DataPath, "welch.txt")));
        }

        void SaveList()
        {
            File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "welch.txt"), welchList.ToArray());
        }
    }
}
