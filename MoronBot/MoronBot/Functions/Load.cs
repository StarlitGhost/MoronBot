using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                moronBot = Program.moronBot;

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
            return null;
        }
    }

    class Unload : Function
    {
        MoronBot moronBot;

        public Unload()
        {
            Help = "unload <function>\t\t- Unloads the specified function.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(unload)$", RegexOptions.IgnoreCase))
            {
                moronBot = Program.moronBot;

                if (message.ParameterList.Count > 0)
                {
                    // Check function exists
                    if (UnloadFromFunctionList(message.ParameterList[0], moronBot.CommandFunctions))
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Function \"" + message.ParameterList[0] + "\" unloaded!", message.ReplyTo) };

                    if (UnloadFromFunctionList(message.ParameterList[0], moronBot.RegexFunctions))
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Function \"" + message.ParameterList[0] + "\" unloaded!", message.ReplyTo) };

                    if (UnloadFromFunctionList(message.ParameterList[0], moronBot.UserListFunctions))
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Function \"" + message.ParameterList[0] + "\" unloaded!", message.ReplyTo) };

                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Function \"" + message.ParameterList[0] + "\" not found!", message.ReplyTo) };
                }

                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't specify a function to unload!", message.ReplyTo) };
            }
            return null;
        }

        bool UnloadFromFunctionList(string funcName, List<IFunction> funcList)
        {
            int index = funcList.FindIndex(cf => cf.Name.ToLowerInvariant() == funcName.ToLowerInvariant());
            if (index >= 0)
            {
                funcList.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
