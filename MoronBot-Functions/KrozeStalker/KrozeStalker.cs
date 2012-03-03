using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Automatic
{
    public class KrozeStalker : Function
    {
        List<string> sayList = new List<string>();
        List<string> doList = new List<string>();

        List<string> userList = new List<string>();

        Random rand = new Random();

        public KrozeStalker()
        {
            Help = "Not a directly callable function; runs automatically, is generally creepy towards Kroze :3";
            Type = Types.UserList;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.PRIVMSGReceived += privmsgReceived;

            userList.Add("Kroze");

            InitLists();
            //sayList = Settings.Instance.FunctionSettings[Name]["SayList"];
            //doList = Settings.Instance.FunctionSettings[Name]["DoList"];
        }

        void privmsgReceived(object sender, BotMessage message)
        {
            if (userList.Contains(message.User.Name) && rand.Next(0, 10) == 0)
            {
                switch (rand.Next(1,3))
                {
                    case 1:
                        FuncInterface.SendResponse(ResponseType.Say, sayList[rand.Next(sayList.Count)], message.ReplyTo);
                        break;
                    case 2:
                        FuncInterface.SendResponse(ResponseType.Do, doList[rand.Next(doList.Count)], message.ReplyTo);
                        break;
                }
            }
        }

        void InitLists()
        {
            sayList.Add("I WUV YOU KROZE! <3");

            doList.Add("stares at Kroze in a very mechanical fashion, desperately wishing it had robot limbs to hug with.");
        }
    }
}
