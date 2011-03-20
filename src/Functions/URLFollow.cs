using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class URLFollow : Function
    {
        public URLFollow(MoronBot moronBot)
        {
            Name = GetName();
            Help = null;
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            Match match = Regex.Match(message.MessageString, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(match.Value);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    string actualURL = response.ResponseUri.Scheme + "://" + response.ResponseUri.Host;
                    string title;

                    Stream responseStream = response.GetResponseStream();
                    System.Text.Encoding encode = System.Text.Encoding.UTF8;
                    StreamReader stream = new StreamReader(responseStream, encode);

                    Char[] buffer = new Char[256];
                    StringBuilder sb = new StringBuilder();

                    int count = stream.Read(buffer, 0, 256);
                    while (count > 0)
                    {
                        sb.Append(buffer);
                        count = stream.Read(buffer, 0, 256);
                    }
                    response.Close();
                    stream.Close();
                    string page = sb.ToString();
                    match = Regex.Match(page, @"<\s*title\s*>(.*?)</title\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        title = "Title: " + match.Groups[1].Value.Trim();
                        title = Regex.Replace(title, @"(\r|\n|)", "", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        title = "No title found";
                    }
                    return new IRCResponse(ResponseType.Say, title + " (at " + actualURL + ")", message.ReplyTo);
                }
                catch (System.Net.WebException ex)
                {
                    Program.form.txtProgLog_Update(ex.Message);
                    return new IRCResponse(ResponseType.Say, "Nothing found at " + match.Value, message.ReplyTo);
                }
                catch (System.UriFormatException ex)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
