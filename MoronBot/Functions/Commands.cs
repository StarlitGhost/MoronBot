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
            Help = "command(s)/help/function(s) (<function>)\t\t- Returns a list of loaded functions, or the help text of a particular function if one is specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(commands?|help|functions?)$", RegexOptions.IgnoreCase))
            {
                moronBot = Program.form.moronBot;

                // Specific function asked for
                if (message.ParameterList.Count > 0)
                {
                    // Check function exists
                    string command = moronBot.CommandList.Find(s => s.IndexOf(message.ParameterList[0], StringComparison.InvariantCultureIgnoreCase) == 0);
                    if (command != null)
                    {
                        // Check function has help text
                        if (moronBot.HelpLibrary[command] != null)
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, moronBot.HelpLibrary[command], message.User.Name) };
                        }
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, "\"" + command + "\" doesn't have any help text specified.", message.User.Name) };
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Notice, "\"" + message.ParameterList[0] + "\" not found, try \"" + message.Command + "\" without parameters to see a list of loaded functions.", message.User.Name) };
                }
                // List of loaded functions asked for
                else
                {
                    List<IRCResponse> responses = new List<IRCResponse>();
                    responses.Add(new IRCResponse(ResponseType.Notice, "Functions loaded are:", message.User.Name));
                    moronBot.CommandList.Sort();
                    string output = moronBot.CommandList[0];
                    for (int i = 1; i < moronBot.CommandList.Count; i++)
                    {
                        output += ", " + moronBot.CommandList[i];
                    }
                    responses.Add(new IRCResponse(ResponseType.Notice, output, message.User.Name));
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
