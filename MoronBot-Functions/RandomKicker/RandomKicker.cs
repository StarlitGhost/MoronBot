using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Automatic
{
    public class RandomKicker : Function
    {
        List<string> userList = new List<string>();

        Random rand = new Random();

        public RandomKicker()
        {
            Help = "Kicks certain users with a 1/5 chance every time they say something.";
            Type = Types.UserList;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.PRIVMSGReceived += privmsgReceived;

            userList.Add("SirGir");
        }

        void privmsgReceived(object sender, BotMessage message)
        {
            if (userList.Contains(message.User.Name) && rand.Next(0, 5) == 0)
            {
                FuncInterface.SendResponse(ResponseType.Raw, "KICK " + message.ReplyTo + " " + message.User.Name + " ::D", "");
            }
        }
    }
}
