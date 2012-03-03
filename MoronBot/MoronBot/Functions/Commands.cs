using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace MoronBot.Functions
{
    class Commands : Function
    {
        MoronBot moronBot;

        public Commands()
        {
            Help = "command(s)/help/function(s) (<function>) - Returns a list of loaded functions, or the help text of a particular function if one is specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }
        
        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(commands?|help|functions?)$", RegexOptions.IgnoreCase))
                return;

            moronBot = Program.moronBot;

            // Specific function asked for
            if (message.ParameterList.Count > 0)
            {
                // Check function exists
                string command = moronBot.CommandList.Find(s => Regex.IsMatch(s, @"^" + Regex.Escape(message.ParameterList[0]) + @"$", RegexOptions.IgnoreCase));
                if (command != null)
                {
                    // Check function has help text
                    if (moronBot.HelpLibrary[command] != null)
                    {
                        FuncInterface.SendResponse(ResponseType.Say, moronBot.HelpLibrary[command], message.ReplyTo);
                        return;
                    }
                    FuncInterface.SendResponse(ResponseType.Say, "\"" + command + "\" doesn't have any help text specified.", message.ReplyTo);
                    return;
                }
                FuncInterface.SendResponse(ResponseType.Say, "\"" + message.ParameterList[0] + "\" not found, try \"" + message.Command + "\" without parameters to see a list of loaded functions.", message.ReplyTo);
                return;
            }
            // List of loaded functions asked for
            else
            {
                FuncInterface.SendResponse(ResponseType.Say, "Functions loaded are:", message.ReplyTo);
                moronBot.CommandList.Sort();
                FuncInterface.SendResponse(ResponseType.Say, string.Join(", ", moronBot.CommandList), message.ReplyTo);
                return;
            }
        }
    }
}
