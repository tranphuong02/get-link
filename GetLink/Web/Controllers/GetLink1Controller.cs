using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace Web.Controllers
{
    public class GetLink1Controller : Controller
    {
        public ActionResult Index()
        {//
           // GetLink();

            var web = new WebClient();
            web.DownloadString(Congifs.DownloadLink);
            //web.UploadValues(Congifs.DownloadLink);

            return View();
        }

        private void GetLink()
        {
            var cookies = new CookieCollection();
            GetLinkHelper.Get(Congifs.LogoutUrl, ref cookies);

            // Login
            var loginHtml = GetLinkHelper.Get(Congifs.DownloadLink, ref cookies);
            var loginFsCsrf = GetLinkHelper.FsLoginCsrfName(GetLinkHelper.HtmlDocument(loginHtml), Congifs.FsCsrfName);
            var postLoginData = string.Format(Congifs.RequestLoginParams, loginFsCsrf);
            var downloadHtml = GetLinkHelper.Post(Congifs.LoginUrl, postLoginData, ref cookies);

            // Post download link
            var fileFsCsrf = GetLinkHelper.FsDownloadCsrfName(GetLinkHelper.HtmlDocument(downloadHtml), Congifs.FsCsrfName);
            var postDownloadData = string.Format(Congifs.RequestDownloadParams, fileFsCsrf);
            GetLinkHelper.Post(Congifs.GetFileLink, postDownloadData, ref cookies);

            // Logout
            GetLinkHelper.Get(Congifs.LogoutUrl, ref cookies);
        }
    }

    public static class Congifs
    {
        public static string LoginUrl = "https://www.fshare.vn/login";
        public static string LogoutUrl = "https://www.fshare.vn/logout";
        public static string GetFileLink = "https://www.fshare.vn/download/get";
        public static string DownloadLink = "https://www.fshare.vn/file/T0R2T7630T/";
        public static string FsCsrfName = "fs_csrf";
        public static string LinkCodeName = "DownloadForm[linkcode]";
        public static string RequestLoginParams = "fs_csrf={0}&LoginForm%5Bemail%5D=tran.phuongvd02%40gmail.com&LoginForm%5Bpassword%5D=trUng+zjn+12%23&LoginForm%5Bcheckloginpopup%5D=0&LoginForm%5BrememberMe%5D=0&yt0=%C4%90%C4%83ng+nh%E1%BA%ADp";
        public static string RequestDownloadParams = "fs_csrf={0}&DownloadForm%5Bpwd%5D=&DownloadForm%5Blinkcode%5D=T0R2T7630T&ajax=download-form&undefined=undefined";
    }

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