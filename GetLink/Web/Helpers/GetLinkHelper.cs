using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace Web.Helpers
{
    public static class GetLinkHelper
    {
        public static HtmlDocument HtmlDocument(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static string FsLoginCsrfName(HtmlDocument html, string inputName)
        {
            var input = html.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == inputName));

            return input == null ? "" : input.Attributes["value"].Value;
        }

        public static string FsDownloadCsrfName(HtmlDocument html, string inputName)
        {
            var input = html.DocumentNode.Descendants("input").LastOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == inputName));

            return input == null ? "" : input.Attributes["value"].Value;
        }

        public static void InitPostRequest(HttpWebRequest request, CookieCollection cookies, string postData)
        {
            // Basic
            request.Method = WebRequestMethods.Http.Post;
            //request.AllowAutoRedirect = true;

            // Cookie
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);

            // IP address
            //request.ServicePoint.BindIPEndPointDelegate = delegate { return new IPEndPoint(IPAddress.Parse("192.168.5.20"), 0); };

            // Headers
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";

            // Content length
            var byteArray = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
        }

        public static string Get(string url, ref CookieCollection cookies)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (response.Cookies != null)
            {
                cookies.Add(response.Cookies);
            }
            if (responseStream == null)
            {
                return "";
            }

            using (var stream = new StreamReader(responseStream))
            {
                return stream.ReadToEnd();
            }
        }

        public static string Get(string url, CookieCollection cookies)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);

            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return "";
            }

            using (var stream = new StreamReader(responseStream))
            {
                Debug.WriteLine("streamGet " + Environment.NewLine + stream.ReadToEnd());
                return stream.ReadToEnd();
            }
        }

        public static string Post(string url, string postData, ref CookieCollection cookies)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                InitPostRequest(request, cookies, postData);

                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (response.Cookies != null)
                {
                    cookies.Add(response.Cookies);
                }
                if (responseStream == null)
                {
                    return "";
                }

                using (var stream = new StreamReader(responseStream))
                {
                    //Debug.WriteLine("stream " + Environment.NewLine + stream.ReadToEnd());
                    return stream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Post ERROR " + ex.Message);
            }

            return "";
        }
    }
}