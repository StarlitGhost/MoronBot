using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Automatic
{
    class KrozeStalker : Function
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

            userList.Add("Kroze");

            InitLists();
            //sayList = Settings.Instance.FunctionSettings[Name]["SayList"];
            //doList = Settings.Instance.FunctionSettings[Name]["DoList"];
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (userList.Contains(message.User.Name) && rand.Next(0, 10) == 0)
            {
                switch (rand.Next(1,3))
                {
                    case 1:
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, sayList[rand.Next(sayList.Count)], message.ReplyTo) };
                    case 2:
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Do, doList[rand.Next(doList.Count)], message.ReplyTo) };
                }
            }
            return null;
        }

        void InitLists()
        {
            sayList.Add("I WUV YOU KROZE! <3");

            doList.Add("stares at Kroze in a very mechanical fashion, desperately wishing it had robot limbs to hug with.");
        }
    }
}
