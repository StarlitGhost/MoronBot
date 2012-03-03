using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Fun
{
    public class MM : Function
    {
        Random rand = new Random();
        List<string> mmList = new List<string>();

        public MM()
        {
            Help = "MM (<number> / add <thing> / list) - Returns a random \"Thing Players can no longer do in an MM game DM'd by Xela'\", a specific one if you give a number, adds a thing to the list, or submits the list to pastebin.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;

            LoadList();
        }

        ~MM()
        {
            SaveList();
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(mm)$", RegexOptions.IgnoreCase))
                return;

            // Return a random thing
            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, mmList[rand.Next(mmList.Count)], message.ReplyTo);
                return;
            }

            if (message.ParameterList[0].ToLower() == "add") // Adding something to the MM list
            {
                string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                int index = mmList.Count + 1;
                mmList.Add(index + ". " + msg);
                SaveList();
                FuncInterface.SendResponse(ResponseType.Say, "Message added at index " + index, message.ReplyTo);
                return;
            }
            else if (message.ParameterList[0] == "list") // Post the list to pastebin, give link
            {
                string list = "";
                foreach (string item in mmList)
                {
                    list += item + "\n";
                }
                string url = URL.Pastebin(list, "M&M List", "10M", "text", "1");
                FuncInterface.SendResponse(ResponseType.Say, "M&M list posted: " + url + " (link expires in 10 mins)", message.ReplyTo);
                return;
            }
            else
            {
                int number = 0;
                if (Int32.TryParse(message.ParameterList[0], out number))
                {
                    number -= 1;
                    if (number >= 0 && number < mmList.Count)
                    {
                        FuncInterface.SendResponse(ResponseType.Say, mmList[number], message.ReplyTo);
                        return;
                    }
                }
                // Number too large or small, or not a number at all
                FuncInterface.SendResponse(ResponseType.Say, "Invalid number, range is 1-" + mmList.Count, message.ReplyTo);
                return;
            }
        }

        void LoadList()
        {
            mmList.AddRange(File.ReadAllLines(Path.Combine(Settings.Instance.DataPath, "mm.txt")));
        }

        void SaveList()
        {
            File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "mm.txt"), mmList.ToArray());
        }
    }
}
