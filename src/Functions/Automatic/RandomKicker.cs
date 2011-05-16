using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Automatic
{
    class RandomKicker : Function
    {
        List<string> userList = new List<string>();

        Random rand = new Random();

        public RandomKicker()
        {
            Help = "Kicks certain users with a 1/5 chance every time they say something.";
            Type = Types.UserList;
            AccessLevel = AccessLevels.Anyone;

            userList.Add("SirGir");
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (userList.Contains(message.User.Name) && rand.Next(0, 5) == 0)
            {
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "KICK " + message.ReplyTo + " " + message.User.Name + " ::D", "") };
            }
            return null;
        }
    }
}
