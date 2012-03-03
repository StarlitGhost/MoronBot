using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using System;

namespace Internet
{
    /// <summary>
    /// A Function which returns the top definition of a given search term from UrbanDictionary.com
    /// </summary>
    /// <suggester>Maerarde</suggester>
    public class Urban : Function
    {
        public Urban()
        {
            Help = "urban <word> - Fetches the definition of the given word from UrbanDictionary.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(urban)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Define what?", message.ReplyTo);
                return;
            }

            string query = HttpUtility.UrlEncode(message.Parameters);
            string url = "http://www.urbandictionary.com/define.php?term=" + query;
                    
            URL.WebPage page;
            try
            {
                page = URL.FetchURL(url);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                FuncInterface.SendResponse(ResponseType.Say, "Couldn't fetch page from UrbanDictionary", message.ReplyTo);
                return;
            }

            Match nameMatch = Regex.Match(page.Page, @"<td class='word'>[\r\n]+(.+?)[\r\n]+</td>");
            Match definitionMatch = Regex.Match(page.Page, @"<div class=""definition"">(.+?)</div><div class=""example"">(.+?)</div>");

            if (!definitionMatch.Success)
            {
                FuncInterface.SendResponse(ResponseType.Say, "No definition found for \"" + message.Parameters + "\"", message.ReplyTo);
                return;
            }

            string word = URL.StripHTML(HttpUtility.HtmlDecode(nameMatch.Groups[1].Value));
            string definition = StringUtils.ReplaceNewlines(URL.StripHTML(HttpUtility.HtmlDecode(definitionMatch.Groups[1].Value)), " | ");
            string example = StringUtils.ReplaceNewlines(URL.StripHTML(HttpUtility.HtmlDecode(definitionMatch.Groups[2].Value)), " | ");

            List<string> responses = new List<string>();

            if (message.Parameters.ToLowerInvariant() == word.ToLowerInvariant())
            {
                responses.Add("Top definition for " + message.Parameters + " (" + url + "):");
            }
            else
            {
                responses.Add("Top definition containing " + message.Parameters + " (" + word + ") (" + url + "):");
            }

            responses.Add(definition);
            responses.Add("Example: " + example);

            FuncInterface.SendResponses(ResponseType.Say, responses, message.ReplyTo);
            return;
        }
    }
}
