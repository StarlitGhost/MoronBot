using System.Text.RegularExpressions;

using CwIRC;

using Gapi;
using System.Collections.Generic;

namespace MoronBot.Functions.Internet
{
    class Google : Function
    {
        public Google()
        {
            Help = "google/search/find <terms>\t- Gives you the first google search result for a given search term.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(google|search|find)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string searchTerm = message.Parameters.Replace("\"", "%22");
                    Gapi.Search.SearchResults searchResults;
                    Gapi.Search.SearchResult searchResult;
                    try
                    {
                        searchResults = Gapi.Search.Searcher.Search(Gapi.Search.SearchType.Web, searchTerm);
                        //Gapi.Search.SearchSafety.Active;
                        //Gapi.Search.S
                        if (searchResults.Items.Length == 0)
                        {
                            return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No results found for \"" + message.Parameters + "\"!", message.ReplyTo) };
                        }

                        searchResult = searchResults.Items[0];
                    }
                    catch (System.Exception ex)
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, ex.GetType().ToString() + " | " + ex.Message, message.ReplyTo) };
                    }
                    string searchContent = Regex.Replace(searchResult.Content, @"<.+?>", "");
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, searchContent + " (" + searchResult.Url + ")", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Search for what?", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
