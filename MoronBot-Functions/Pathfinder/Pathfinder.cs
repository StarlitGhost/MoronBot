using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Fun
{
    public class Pathfinder : Function
    {
        Random rand = new Random();
        List<string> pfList = new List<string>();

        Object fileLock = new Object();

        public Pathfinder()
        {
            Help = "pf (<number> / add <thing> / list) - Returns a random thing from the DesertBus Survivors' Pathfinder game, a specific one if you give a number, adds a thing to the list, or submits the list to pastebin.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadList();
        }

        ~Pathfinder()
        {
            SaveList();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(pf)$", RegexOptions.IgnoreCase))
                return null;

            if (message.ParameterList.Count == 0)
            {
                if (pfList.Count > 0)
                {
                    // Return a random thing
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, pfList[rand.Next(pfList.Count)], message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "There is currently no Pathfinder data!", message.ReplyTo) };
                }
            }

            switch (message.ParameterList[0].ToLower())
            {
                case "add": // Adding something to the list
                    string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);

                    int index;
                    lock (fileLock)
                    {
                        index = pfList.Count + 1;
                        pfList.Add(index + ". " + msg);
                    }

                    SaveList();
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Message added at index " + index, message.ReplyTo) };

                case "list": // Post the list to pastebin, give link
                    string list = "";
                    foreach (string item in pfList)
                    {
                        list += item + "\n";
                    }
                    string url = URL.Pastebin(list, "DesertBus Survivors' Pathfinder List", "10M", "text", "1");
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "DB Pathfinder list posted: " + url + " (link expires in 10 mins)", message.ReplyTo) };

                default:
                    int number = 0;
                    if (Int32.TryParse(message.ParameterList[0], out number))
                    {
                        number -= 1;
                        if (number >= 0 && number < pfList.Count)
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, pfList[number], message.ReplyTo) };
                        }
                    }
                    // Number too large or small, or not a number at all
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + pfList.Count, message.ReplyTo) };
            }
        }

        void LoadList()
        {
            lock (fileLock)
            {
                pfList.AddRange(File.ReadAllLines(Path.Combine(Settings.Instance.DataPath, "pathfinder.txt")));
            }
        }

        void SaveList()
        {
            lock (fileLock)
            {
                File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "pathfinder.txt"), pfList.ToArray());
            }
        }
    }
}
