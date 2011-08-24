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

            LoadList();
        }

        ~LP()
        {
            SaveList();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(lp)$", RegexOptions.IgnoreCase))
            {
                // Specific thing requested
                if (message.ParameterList.Count > 0)
                {
                    if (message.ParameterList[0].ToLower() == "add") // Adding something to the LP list
                    {
                        string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                        int index = lpList.Count + 1;
                        lpList.Add(index + ". " + msg);
                        SaveList();
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Message added at index " + index, message.ReplyTo) };
                    }
                    else if (message.ParameterList[0] == "list") // Post the list to pastebin, give link
                    {
                        string list = "";
                        foreach (string item in lpList)
                        {
                            list += item + "\n";
                        }
                        string url = URL.Pastebin(list, "Let's Play List", "10M", "text", "1");
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Let's Play list posted: " + url + " (link expires in 10 mins)", message.ReplyTo) };
                    }
                    else
                    {
                        int number = 0;
                        if (Int32.TryParse(message.ParameterList[0], out number))
                        {
                            number -= 1;
                            if (number >= 0 && number < lpList.Count)
                            {
                                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, lpList[number], message.ReplyTo) };
                            }
                        }
                        // Number too large or small, or not a number at all
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + lpList.Count, message.ReplyTo) };
                    }
                }
                // No specific thing requested
                else
                {
                    if (lpList.Count > 0)
                    {
                        // Return a random thing
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, lpList[rand.Next(lpList.Count)], message.ReplyTo) };
                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "There is currently no LP data!", message.ReplyTo) };
                    }
                }
            }
            else
            {
                return null;
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
