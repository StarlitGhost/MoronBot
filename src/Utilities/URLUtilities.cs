using System;
using System.IO;
using System.Net;
using System.Text;

namespace MoronBot.Utilities
{
    static class URL
    {
        public struct WebPage
        {
            public string Domain;
            public string Page;
        }

        public static WebPage FetchURL(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

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

                WebPage webPage = new WebPage();

                webPage.Domain = response.ResponseUri.Scheme + "://" + response.ResponseUri.Host;
                webPage.Page = sb.ToString();

                response.Close();
                stream.Close();

                return webPage;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
