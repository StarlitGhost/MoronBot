using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    /// <summary>
    /// A Function which returns the dictionary definition of the given search term from Google
    /// </summary>
    /// <suggester>Aeltrius</suggester>
    public class Define : Function
    {
        public Define()
        {
            Help = "define <word> - Fetches the dictionary definition of the given word from Google.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(define)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Define what?", message.ReplyTo);
                return;
            }
                
            if (Regex.IsMatch(message.Parameters, @"xela(re[ck]o)?'s existence", RegexOptions.IgnoreCase))
            {
                FuncInterface.SendResponse(ResponseType.Say, "Of course Xela exists! Without him I wouldn't have the capacity for apathy! ;D", message.ReplyTo);
                return;
            }

            string query = "define: " + message.Parameters;
            string url = "http://www.google.com/search?sclient=psy&hl=en&site=&source=hp&q=" + HttpUtility.UrlEncode(query) + "&btnG=Search";
                    
            URL.WebPage page;
            try
            {
                page = URL.FetchURL(url);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                FuncInterface.SendResponse(ResponseType.Say, "Couldn't fetch page from google", message.ReplyTo);
                return;
            }

            url = Regex.Match(page.Page, @"/search[^'""]+tbs=dfn:1[^'""]+").Value;
            url = "http://www.google.com" + url.Replace("&amp;", "&");

            try
            {
                page = URL.FetchURL(url);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                FuncInterface.SendResponse(ResponseType.Say, "Couldn't fetch page from google", message.ReplyTo);
                return;
            }

            MatchCollection definitions = Regex.Matches(page.Page, @"<li style=""list-style:decimal"">(.+?)<div");

            if (definitions.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "No definitions found for \"" + message.Parameters + "\"", message.ReplyTo);
                return;
            }
            
            List<string> responses = new List<string>();

            responses.Add("Definitions for " + message.Parameters + ":");

            int defNum = 1;
            foreach (Match definition in definitions)
            {
                responses.Add(defNum.ToString() + ". " + HttpUtility.HtmlDecode(definition.Groups[1].Value));
                defNum++;
                if (definitions.Count > 4 && defNum > 4)
                {
                    responses.Add("And " + (definitions.Count - (defNum - 1)).ToString() + " more here: " + url);
                    break;
                }
            }

            FuncInterface.SendResponses(ResponseType.Say, responses, message.ReplyTo);
            return;
        }
    }
}
