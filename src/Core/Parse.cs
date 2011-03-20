using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CwIRC
{
    public class Parse
    {
        public static List<string> UserList(List<string> p_IRCMessageList)
        {
            List<string> userList = new List<string>();
            userList.Add(p_IRCMessageList[5].TrimStart(':'));
            for (int i = 6; i < p_IRCMessageList.Count; ++i)
            {
                if (p_IRCMessageList[i].Length > 0)
                {
                    userList.Add(p_IRCMessageList[i]);
                }
            }
            return userList;
        }
    }
}
