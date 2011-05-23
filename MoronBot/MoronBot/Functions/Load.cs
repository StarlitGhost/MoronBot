using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using CwIRC;
using MBFunctionInterface;

namespace MoronBot.Functions
{
    class Load : Function
    {
        MoronBot moronBot;

        public Load()
        {
            Help = "load <function>\t\t- Loads the specified function.";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(load)$", RegexOptions.IgnoreCase))
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
            }
            else
            {
                return null;
            }
        }
    }

    class Unload : Function
    {
        MoronBot moronBot;

        public Unload()
        {
            Help = "unload <function>\t\t- Unloads the specified function.";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(unload)$", RegexOptions.IgnoreCase))
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
            }
            else
            {
                return null;
            }
        }
    }
}
