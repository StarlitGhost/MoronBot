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
                    string query = message.Parameters;
                    string url = "http://www.urbandictionary.com/define.php?term=" + HttpUtility.UrlEncode(query);
                    
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

                    Match definition = Regex.Match(page.Page, @"<div class=""definition"">(.+?)</div><div class=""example"">(.+?)</div>");

                    if (definition.Success)
                    {
                        List<IRCResponse> responses = new List<IRCResponse>();

                        responses.Add(new IRCResponse(
                            ResponseType.Say,
                            "Top definition for " + message.Parameters + " (" + url + "):",
                            message.ReplyTo));
                        responses.Add(new IRCResponse(
                            ResponseType.Say,
                            Regex.Replace(Regex.Replace(HttpUtility.HtmlDecode(definition.Groups[1].Value), @"<.*?>", String.Empty), @"[\r\n]+", " | "),
                            message.ReplyTo));
                        responses.Add(new IRCResponse(
                            ResponseType.Say,
                            "Example: " + Regex.Replace(Regex.Replace(HttpUtility.HtmlDecode(definition.Groups[2].Value), @"<.*?>", String.Empty), @"[\r\n]+", " | "),
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
