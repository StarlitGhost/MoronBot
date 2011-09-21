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
    public class Urban : Function
    {
        public Urban()
        {
            Help = "urban <word> - Fetches the definition of the given word from UrbanDictionary.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(urban)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
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
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Couldn't fetch page from UrbanDictionary", message.ReplyTo) };
                    }

                    Match nameMatch = Regex.Match(page.Page, @"<td class='word'>[\r\n]+(.+?)[\r\n]+</td>");
                    Match definitionMatch = Regex.Match(page.Page, @"<div class=""definition"">(.+?)</div><div class=""example"">(.+?)</div>");

                    if (definitionMatch.Success)
                    {
                        string word = URL.StripHTML(HttpUtility.HtmlDecode(nameMatch.Groups[1].Value));
                        string definition = StringUtils.ReplaceNewlines(URL.StripHTML(HttpUtility.HtmlDecode(definitionMatch.Groups[1].Value)), " | ");
                        string example = StringUtils.ReplaceNewlines(URL.StripHTML(HttpUtility.HtmlDecode(definitionMatch.Groups[2].Value)), " | ");

                        List<IRCResponse> responses = new List<IRCResponse>();

                        if (message.Parameters.ToLowerInvariant() == word.ToLowerInvariant())
                        {
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                "Top definition for " + message.Parameters + " (" + url + "):",
                                message.ReplyTo));
                        }
                        else
                        {
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                "Top definition containing " + message.Parameters + " (" + word + ") (" + url + "):",
                                message.ReplyTo));
                        }

                        responses.Add(new IRCResponse(
                            ResponseType.Say,
                            definition,
                            message.ReplyTo));
                        responses.Add(new IRCResponse(
                            ResponseType.Say,
                            "Example: " + example,
                            message.ReplyTo));

                        return responses;
                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No definition found for \"" + message.Parameters + "\"", message.ReplyTo) };
                    }
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Define what?", message.ReplyTo) };
                }
            }

            return null;
        }
    }
}
