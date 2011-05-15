using System;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Commands : Function
    {
        public Commands(MoronBot moronBot)
        {
            Name = GetName();
            Help = "command(s)/help/function(s) (<function>)\t\t- Returns a list of loaded functions, or the help text of a particular function if one is specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(commands?|help|functions?)$", RegexOptions.IgnoreCase))
            {
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
                            moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, moronBot.HelpLibrary[command], message.User.Name));
                            return;
                        }
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, "\"" + command + "\" doesn't have any help text specified.", message.User.Name));
                        return;
                    }
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, "\"" + message.ParameterList[0] + "\" not found, try \"" + message.Command + "\" without parameters to see a list of loaded functions.", message.User.Name));
                    return;
                }
                // List of loaded functions asked for
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, "Functions loaded are:", message.User.Name));
                    moronBot.CommandList.Sort();
                    string output = moronBot.CommandList[0];
                    for (int i = 1; i < moronBot.CommandList.Count; i++)
                    {
                        output += ", " + moronBot.CommandList[i];
                    }
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Notice, output, message.User.Name));
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
