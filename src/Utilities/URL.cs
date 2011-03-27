using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Bitly;

namespace MoronBot.Utilities
{
    // Utility class to hold commonly needed helper functions to do with URLs and general web stuff.
    static class URL
    {
        public struct WebPage
        {
            public string Domain;
            public string Page;
        }

        /// <summary>
        /// Fetches the page a URL refers to, and returns a WebPage struct.
        /// </summary>
        /// <param name="url">The URL to fetch.</param>
        /// <returns>A WebPage struct containing the domain and the page itself.</returns>
        public static WebPage FetchURL(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                System.Text.Encoding encode = System.Text.Encoding.UTF8;
                StreamReader stream = new StreamReader(responseStream, encode);

                StringBuilder sb = new StringBuilder();

                sb.Append(stream.ReadToEnd());

                WebPage webPage = new WebPage();

                webPage.Domain = response.ResponseUri.Scheme + "://" + response.ResponseUri.Host;
                webPage.Page = sb.ToString();

                response.Close();
                stream.Close();

                return webPage;
            }
            catch (System.Exception ex)
            {
                // Propagate exceptions upwards
                throw ex;
            }
        }

        public static string Shorten(string url)
        {
            // Check that the 'URL' given, is actually a URL
            Match match = Regex.Match(url, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // Use the Bitly API to shorten the given URL
                return API.Bit("tyranicmoron", "R_2cec505899bffdf2f88e0a15953661e6", match.Value, "Shorten");
            }
            return null;
        }
    }
}
