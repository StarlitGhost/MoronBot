using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MBUtilities
{
    // Utility class to hold commonly needed helper functions to do with URLs and general web stuff.
    public static class URL
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
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (Regex.IsMatch(response.ContentType, @"^(text/.*|application/((rss|atom|rdf)\+)?xml(;.*)?)$"))
                {
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

                return new WebPage();
            }
            catch (System.Exception ex)
            {
                // Propagate exceptions upwards
                throw ex;
            }
        }

        static bool CheckValidationResult(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static Stream SendToServer(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            return webResponse.GetResponseStream();
        }
        public static Stream SendToServer(string url, string text)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(text);
            writer.Close();

            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            return webResponse.GetResponseStream();
        }

        public static string ReceiveFromServer(Stream responseStream)
        {
            Encoding encode = Encoding.UTF8;
            StreamReader stream = new StreamReader(responseStream, encode);

            StringBuilder sb = new StringBuilder();
            sb.Append(stream.ReadToEnd());
            return sb.ToString();
        }

        public static string Shorten(string url)
        {
            // Check that the 'URL' given, is actually a URL
            Match match = Regex.Match(url, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // Use the Bitly API to shorten the given URL
                //return API.Bit("tyranicmoron", "R_2cec505899bffdf2f88e0a15953661e6", match.Value, "Shorten");
                return ShortenGoogl(match.Value);
            }
            return null;
        }

        static string ShortenGoogl(string url)
        {
            const string postFormat = @"{{""longUrl"":""{0}""}}";

            string key = GooglKey();
            string post = String.Format(postFormat, url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?fields=id&key=" + key);
            request.ServicePoint.Expect100Continue = false;
            request.Method = "POST";
            request.UserAgent = "MoronBot";
            request.ContentLength = post.Length;
            request.ContentType = "application/json";
            request.Headers.Add("Cache-Control", "no-cache");

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                requestStream.Write(postBuffer, 0, postBuffer.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                string json = responseReader.ReadToEnd();
                return Regex.Match(json, @"""id"":(\s+)?""([^""]*)""").Groups[2].Value;
            }
        }

        static string GooglKey()
        {
            return "AIzaSyCU7yKR6eTkme1cTUqFoSJxhG-v83trPy4";
        }

        public static string Pastebin(string text, string title = "", string expire = "10M", string format = "text", string privacy = "1")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://pastebin.com/api/api_post.php");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string api_dev_key = "103e700947c8d6782b3fb99c85ae4d9f";
            string api_option = "paste";
            string api_paste_code = text;

            string api_user_key = "";
            string api_paste_name = title;
            string api_paste_format = format;
            string api_paste_private = privacy;
            string api_paste_expire_date = expire;

            string postData =
                "api_dev_key=" + HttpUtility.UrlEncode(api_dev_key) +
                "&api_option=" + HttpUtility.UrlEncode(api_option) +
                "&api_paste_code=" + HttpUtility.UrlEncode(api_paste_code) +

                "&api_user_key=" + HttpUtility.UrlEncode(api_user_key) +
                "&api_paste_name=" + HttpUtility.UrlEncode(api_paste_name) +
                "&api_paste_format=" + HttpUtility.UrlEncode(api_paste_format) +
                "&api_paste_private=" + HttpUtility.UrlEncode(api_paste_private) +
                "&api_paste_expire_date=" + HttpUtility.UrlEncode(api_paste_expire_date);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            request.ContentLength = bytes.Length;

            using (Stream writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            string result;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static string StripHTML(string text)
        {
            return Regex.Replace(text, @"<.*?>", String.Empty);
        }
    }
}
