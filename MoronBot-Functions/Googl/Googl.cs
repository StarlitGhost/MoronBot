using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Internet
{
    /// <summary>
    /// A Function which returns a shortened version of the given url, via Goo.gl.
    /// </summary>
    public class Googl : Function
    {
        public Googl()
        {
            Help = "googl/shorten <url> - Gives you a shortened version of a url, via Goo.gl";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^(goo\.?gl|shorten)$", RegexOptions.IgnoreCase))
                return;
            
            if (message.ParameterList.Count == 0)
            {
                // No URL given
                FuncInterface.SendResponse(ResponseType.Say, "You didn't give a URL to shorten!", message.ReplyTo);
                return;
            }

            string shortURL = URL.Shorten(message.Parameters);

            if (shortURL == null)
            {
                FuncInterface.SendResponse(ResponseType.Say, "No URL detected in your message.", message.ReplyTo);
                return;
            }

            FuncInterface.SendResponse(ResponseType.Say, shortURL, message.ReplyTo);
            return;
        }
    }
}
