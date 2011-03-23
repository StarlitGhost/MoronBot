using System.Collections.Generic;
using CwIRC;

namespace MoronBot.Functions
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

    abstract class Function
    {
        public string Name;

        public string Help;

        public Types Type = Types.Command;

        public AccessLevels AccessLevel = AccessLevels.Anyone;

        public List<string> AccessList = new List<string>();

        abstract public void GetResponse(BotMessage message, MoronBot moronBot);

        protected string GetName()
        {
            return this.GetType().ToString().Split('.')[2];
        }
    }
}
