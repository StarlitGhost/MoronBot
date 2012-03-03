using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

using Gapi;

namespace Internet
{
    /// <summary>
    /// A Function which returns the top Google search result for a given search term.
    /// </summary>
    public class Google : Function
    {
        public Google()
        {
            Help = "google/search/find <terms> - Gives you the first google search result for a given search term.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(google|search|find)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Search for what?", message.ReplyTo);
                return;
            }

            string searchTerm = message.Parameters.Replace("\"", "%22");
            Gapi.Search.SearchResults searchResults;
            Gapi.Search.SearchResult searchResult;
            try
            {
                searchResults = Gapi.Search.Searcher.Search(Gapi.Search.SearchType.Web, searchTerm);

                if (searchResults.Items.Length == 0)
                {
                    FuncInterface.SendResponse(ResponseType.Say, "No results found for \"" + message.Parameters + "\"!", message.ReplyTo);
                    return;
                }

                searchResult = searchResults.Items[0];
            }
            catch (System.Exception ex)
            {
                FuncInterface.SendResponse(ResponseType.Say, ex.GetType().ToString() + " | " + ex.Message, message.ReplyTo);
                return;
            }
            string searchContent = HttpUtility.HtmlDecode(URL.StripHTML(searchResult.Content));
            FuncInterface.SendResponse(ResponseType.Say, searchContent + " (" + searchResult.Url + ")", message.ReplyTo);
            return;
        }
    }
}
