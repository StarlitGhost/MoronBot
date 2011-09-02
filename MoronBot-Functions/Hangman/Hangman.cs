using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

namespace Fun
{
    public class Hangman : Function
    {
        public Hangman()
        {
            Help = "h(ang)m(an) <command> (<params>) - A game of hangman. Sub-commands are as follows: start, stop, guess";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(h(ang)?m(an)?)$", RegexOptions.IgnoreCase))
            {

            }
            throw new NotImplementedException();
        }
    }
}
