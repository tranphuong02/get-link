using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class GetLinkController : Controller
    {
        private string LoginUrl = "https://www.fshare.vn/login";
        private string LogoutUrl = "https://www.fshare.vn/logout";
        private string DownloadLink = "https://www.fshare.vn/file/T0R2T7630T/";
        private string GetFileLink = "https://www.fshare.vn/download/get";
        private string Email = HttpUtility.UrlEncode("tran.phuongvd02@gmail.com");
        private string Password = HttpUtility.UrlEncode("trUng zjn 12#");
        private string FsCsrfName = "fs_csrf";
        private string LinkCodeName = "DownloadForm[linkcode]";
        private string RequestLoginParams = "fs_csrf={0}&LoginForm%5Bemail%5D={1}&LoginForm%5Bpassword%5D={2}&LoginForm%5Bcheckloginpopup%5D=0&LoginForm%5BrememberMe%5D=0&LoginForm%5BrememberMe%5D=1&yt0=%C4%90%C4%83ng+nh%E1%BA%ADp";
        private string RequestDownloadParams = "fs_csrf={0}&DownloadForm%5Bpwd%5D=&DownloadForm%5Blinkcode%5D={1}&ajax=download-form&undefined=undefined";

        public ActionResult Index()
        {
            var link1 = GetLink();
            return View();
        }

        #region Get link

        private string GetFsCsrf(HtmlDocument html)
        {
            var fsCsrfNode = html.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == FsCsrfName));

            return fsCsrfNode == null ? "" : fsCsrfNode.Attributes["value"].Value;
        }

        private string GetlinkCode(HtmlDocument html)
        {
            var fsCsrfNode = html.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == LinkCodeName));

            return fsCsrfNode == null ? "" : fsCsrfNode.Attributes["value"].Value;
        }

        private static HtmlDocument HtmlDocument(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private static string GetPageContent(string url, ref CookieCollection cookies)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            
            var response = (HttpWebResponse)request.GetResponse();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                cookies = response.Cookies;
                return stream.ReadToEnd();
            }
        }

        private static void Logout(string url)
        {
           WebRequest.Create(url).GetResponse();
        }

        private static FileResponse Post(string url, string postData, ref CookieCollection cookies)
        {
            try
            {
                var getRequest = (HttpWebRequest)WebRequest.Create(url);
                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer.Add(cookies);
                getRequest.Method = WebRequestMethods.Http.Post;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.AllowWriteStreamBuffering = true;
                getRequest.AllowAutoRedirect = true;
                getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                getRequest.ContentType = "application/x-www-form-urlencoded";

                var byteArray = Encoding.ASCII.GetBytes(postData);
                getRequest.ContentLength = byteArray.Length;

                var newStream = getRequest.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();

                var response = (HttpWebResponse)getRequest.GetResponse();
                var result = new FileResponse();
                using (var stream = new StreamReader(response.GetResponseStream()))
                {

                    Debug.WriteLine("------------------------------------------------------------------");
                    Debug.WriteLine(stream.ReadToEnd());

                    cookies = response.Cookies;
                    result.IsSuccess = response.StatusCode != HttpStatusCode.BadRequest;

                    if (HttpStatusCode.BadRequest == response.StatusCode)
                    {
                        result = new FileResponse
                        {
                            IsSuccess = false,
                            Content = stream.ReadToEnd()
                        };
                    }

                    result = new FileResponse
                    {
                        IsSuccess = true,
                        Content = JsonConvert.DeserializeObject<SuccessResponse>(stream.ReadToEnd()).url
                    };
                }
                return result;
            }
            catch (System.Exception ex)
            {
                return new FileResponse();
            }
        } 

        private string GetLink()
        {
            var cookies = new CookieCollection();

            // Login
            var html = GetPageContent(LoginUrl, ref cookies);
            var fsCsrf = GetFsCsrf(HtmlDocument(html));
            var postData = string.Format(RequestLoginParams, fsCsrf, Email, Password);

            // Post login
            Post(LoginUrl, postData, ref cookies);

            // Get link
            var html1 = GetPageContent(DownloadLink, ref cookies);
            var htmlDocument = HtmlDocument(html1);
            var fsCsrf1 = GetFsCsrf(htmlDocument);
            var linkCode = GetlinkCode(htmlDocument);
            var postData1 = string.Format(RequestDownloadParams, fsCsrf1, linkCode);
            var result = Post(GetFileLink, postData1, ref cookies);

            // Logout
            Logout(LogoutUrl);

            return "";
        }

        #endregion
    }

    public class FileResponse
    {
        public string Content { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class SuccessResponse
    {
        public string url { get; set; }
        public bool wait_time { get; set; }
    }
}