using System.Collections.Generic;

using CwIRC;

namespace MBFunctionInterface
{
    public enum Types
    {
        UserList,
        Regex,
        Command
    }

    public enum AccessLevels
    {
        SameHost,
        UserList,
        Anyone
    }

    public interface IFunction
    {
        /// <summary>
        /// The name of the function. Used for outputting a list of loaded functions, and to fetch help text.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Used to tell the user what a function does, and how to use it (if it is directly callable).
        /// Try to keep it short, but a paragraph or 2 is fine. Bear in mind that line breaks don't work, though.
        /// </summary>
        string Help { get; set; }

        /// <summary>
        /// The type of function this is. Determines how the function will be executed.
        ///  - UserList functions are always executed, bypassing the global ignore list.
        ///     This type is intended for functions with their own user access management.
        ///  - Regex functions are called on every message.
        ///     Not called for users on the global ignore list.
        ///  - Command functions are only called if the message starts with one of the direct command strings: "|", "nick ", "nick, ", "nick: "
        ///     Not called for users on the global ignore list.
        /// </summary>
        Types Type { get; set; }

        /// <summary>
        /// Who can access this function.
        ///  - SameHost is anyone with the same hostmask as the bot.
        ///  - UserList is anyone on the function's access list.
        ///  - Anyone is, well... anyone.
        /// At the moment you would have to enforce these in the function itself.
        /// </summary>
        AccessLevels AccessLevel { get; set; }

        /// <summary>
        /// List of users allowed to call this function.
        /// At the moment you would have to enforce this in the function itself.
        /// </summary>
        List<string> AccessList { get; set; }

        /// <summary>
        /// The method the bot calls for any messages matching your function's type.
        /// </summary>
        /// <param name="message">A message for you to deal with</param>
        /// <param name="moronBot">The bot itself - may need to remove this from here</param>
        /// <returns>A list of IRCResponses to send back to the channel/user</returns>
        List<IRCResponse> GetResponse(BotMessage message);
    }

    public abstract class Function : IFunction
    {
        public string Name { get; set; }
        public string Help { get; set; }
        public Types Type { get; set; }
        public AccessLevels AccessLevel { get; set; }
        public List<string> AccessList { get; set; }

        public abstract List<IRCResponse> GetResponse(BotMessage message);

        /// <summary>
        /// Helper function to get the name of 'this' class; ie, one implementing this abstract base.
        /// Strips away the inheritance/namespace/etc hierarchy normally present in the Class Type ToString output,
        /// leaving just the name of the class in isolation.
        /// </summary>
        /// <returns>'this' class' name</returns>
        private string GetName()
        {
            string[] name = this.GetType().ToString().Split('.');
            return name[name.Length - 1];
        }

        /// <summary>
        /// Default constructor, just uses GetName to set the function's name.
        /// </summary>
        public Function()
        {
            Name = GetName();
            Help = "";
            AccessList = new List<string>();
            AccessLevel = AccessLevels.Anyone;
            Type = Types.Command;
        }
    }
}
