using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Bitly;
using System;

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
            const string postFormat = "&user=toolbar@google.com&url={0}&auth_token={1}";

            string token = GenerateAuthToken(url);
            string post = String.Format(postFormat, HttpUtility.UrlEncode(url), token);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://goo.gl/api/url");
            request.ServicePoint.Expect100Continue = false;
            request.Method = "POST";
            request.UserAgent = "toolbar";
            request.ContentLength = post.Length;
            request.ContentType = "application/x-www-form-urlencoded";
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
                return Regex.Match(json, @"{""short_url"":""([^""]*)""[,}]").Groups[1].Value;
            }
        }

        static string GenerateAuthToken(string b)
        {
            long i = __e(b);

            i = i >> 2 & 1073741823;
            i = i >> 4 & 67108800 | i & 63;
            i = i >> 4 & 4193280 | i & 1023;
            i = i >> 4 & 245760 | i & 16383;

            long h = __f(b);
            long k = (i >> 2 & 15) << 4 | h & 15;
            k |= (i >> 6 & 15) << 12 | (h >> 8 & 15) << 8;
            k |= (i >> 10 & 15) << 20 | (h >> 16 & 15) << 16;
            k |= (i >> 14 & 15) << 28 | (h >> 24 & 15) << 24;

            return "7" + __d(k);
        }

        static long __c(long a, long b, long c)
        {
            long l = 0;
            l += (a & 4294967295);
            l += (b & 4294967295);
            l += (c & 4294967295);

            return l;
        }

        static long __c(long a, long b, long c, long d)
        {
            long l = 0;
            l += (a & 4294967295);
            l += (b & 4294967295);
            l += (c & 4294967295);
            l += (d & 4294967295);

            return l;
        }

        static string __d(long l)
        {
            string ll = l.ToString();
            string m = (l > 0 ? l : l + 4294967296).ToString();
            bool n = false;
            long o = 0;

            for (int p = m.Length - 1; p >= 0; --p)
            {
                long q = Int64.Parse(m[p].ToString());

                if (n)
                {
                    q *= 2;
                    o += (long)Math.Floor((double)q / 10) + q % 10;
                }
                else
                {
                    o += q;
                }

                n = !n;
            }

            long mm = o % 10;

            o = 0;

            if (mm != 0)
            {
                o = 10 - mm;

                if (ll.Length % 2 == 1)
                {
                    if (o % 2 == 1) o += 9;
                    o /= 2;
                }
            }

            m = o.ToString();
            m += ll;

            return m;
        }

        static long __e(string l)
        {
            long m = 5381;
            for (int o = 0; o < l.Length; o++)
            {
                m = __c(m << 5, m, (long)l[o]);
            }
            return m;
        }

        static long __f(string l)
        {
            long m = 0;
            for (int o = 0; o < l.Length; o++)
            {
                m = __c(l[o], m << 6, m << 16, -m);
            }
            return m;
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
    }
}
