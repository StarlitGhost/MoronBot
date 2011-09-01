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
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(commands?|help|functions?)$", RegexOptions.IgnoreCase))
            {
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
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, moronBot.HelpLibrary[command], message.ReplyTo) };
                        }
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "\"" + command + "\" doesn't have any help text specified.", message.ReplyTo) };
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "\"" + message.ParameterList[0] + "\" not found, try \"" + message.Command + "\" without parameters to see a list of loaded functions.", message.ReplyTo) };
                }
                // List of loaded functions asked for
                else
                {
                    List<IRCResponse> responses = new List<IRCResponse>();
                    responses.Add(new IRCResponse(ResponseType.Say, "Functions loaded are:", message.ReplyTo));
                    moronBot.CommandList.Sort();
                    string output = moronBot.CommandList[0];
                    for (int i = 1; i < moronBot.CommandList.Count; i++)
                    {
                        output += ", " + moronBot.CommandList[i];
                    }
                    responses.Add(new IRCResponse(ResponseType.Say, output, message.ReplyTo));
                    return responses;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
