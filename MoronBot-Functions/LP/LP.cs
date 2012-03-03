using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Fun
{
    public class LP : Function
    {
        Random rand = new Random();
        List<string> lpList = new List<string>();

        public LP()
        {
            Help = "LP (<number>) - Returns a random \"Thing from a DesertBus Chat Let's Play\", or a specific one if you add a number.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;

            LoadList();
        }

        ~LP()
        {
            SaveList();
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(lp)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                if (lpList.Count > 0)
                {
                    // Return a random thing
                    FuncInterface.SendResponse(ResponseType.Say, lpList[rand.Next(lpList.Count)], message.ReplyTo);
                    return;
                }
                else
                {
                    FuncInterface.SendResponse(ResponseType.Say, "There is currently no LP data!", message.ReplyTo);
                    return;
                }
            }

            if (message.ParameterList[0].ToLower() == "add") // Adding something to the LP list
            {
                string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                int index = lpList.Count + 1;
                lpList.Add(index + ". " + msg);
                SaveList();
                FuncInterface.SendResponse(ResponseType.Say, "Message added at index " + index, message.ReplyTo);
                return;
            }
            else if (message.ParameterList[0] == "list") // Post the list to pastebin, give link
            {
                string list = "";
                foreach (string item in lpList)
                {
                    list += item + "\n";
                }
                string url = URL.Pastebin(list, "Let's Play List", "10M", "text", "1");
                FuncInterface.SendResponse(ResponseType.Say, "Let's Play list posted: " + url + " (link expires in 10 mins)", message.ReplyTo);
                return;
            }
            else
            {
                int number = 0;
                if (Int32.TryParse(message.ParameterList[0], out number))
                {
                    number -= 1;
                    if (number >= 0 && number < lpList.Count)
                    {
                        FuncInterface.SendResponse(ResponseType.Say, lpList[number], message.ReplyTo);
                        return;
                    }
                }
                // Number too large or small, or not a number at all
                FuncInterface.SendResponse(ResponseType.Say, "Invalid number, range is 1-" + lpList.Count, message.ReplyTo);
                return;
            }
        }

        void LoadList()
        {
            lpList.AddRange(File.ReadAllLines(Path.Combine(Settings.Instance.DataPath, "lp.txt")));
        }

        void SaveList()
        {
            File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "lp.txt"), lpList.ToArray());
        }
    }
}
