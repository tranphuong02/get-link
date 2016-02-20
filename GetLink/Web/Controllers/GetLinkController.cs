using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
namespace Web.Controllers
{
    public class GetLinkController : Controller
    {
        private string LoginUrl = "https://www.fshare.vn/login";
        private string Email = HttpUtility.UrlEncode("tran.phuongvd02@gmail.com");
        private string Password = HttpUtility.UrlEncode("trUng zjn 12#");
        private string FsCsrfName = "fs_csrf";
        private string RequestLoginParams = "fs_csrf={0}&LoginForm%5Bemail%5D={1}&LoginForm%5Bpassword%5D={2}&LoginForm%5Bcheckloginpopup%5D=0&LoginForm%5BrememberMe%5D=0&LoginForm%5BrememberMe%5D=1&yt0=%C4%90%C4%83ng+nh%E1%BA%ADp";

        public ActionResult Index()
        {
            //var link =  GetLink();
            var link1 = GetLink1();
            return View();
        }

        #region Get link
        private string GetLink()
        {
            var webClient = new WebClient();
            var loginHtml = LoginPageContent(webClient);
            var fsCsrf = GetFsCsrf(loginHtml);
            Login(webClient, fsCsrf);
            return "";
        }

        private HtmlDocument LoginPageContent(WebClient webClient)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(webClient.DownloadString(LoginUrl));
            return htmlDocument;
        }

        private string GetFsCsrf(HtmlDocument loginHtml)
        {
            var fsCsrfNode = loginHtml.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == FsCsrfName));

            return fsCsrfNode == null ? "" : fsCsrfNode.Attributes["value"].Value;
        }

        private void Login(WebClient webClient, string fsCsrf)
        {
            var loginRequestParams = string.Format(RequestLoginParams, fsCsrf, Email, Password);
            var result = webClient.UploadString(LoginUrl, "POST", loginRequestParams);

            var bb = result;
        }
        #endregion

        #region Get link 1
        private HtmlDocument LoginPageContent1(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private string GetLink1()
        {
            CookieCollection cookies = new CookieCollection();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LoginUrl);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            //Get the response from the server and save the cookies from the first request..
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var html = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                html =  sr.ReadToEnd();
            }
            cookies = response.Cookies;

            var fsCsrf = GetFsCsrf(LoginPageContent1(html));
            string postData = string.Format(RequestLoginParams, fsCsrf, Email, Password);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(LoginUrl);
            getRequest.CookieContainer = new CookieContainer();
            getRequest.CookieContainer.Add(cookies); //recover cookies First request
            getRequest.Method = WebRequestMethods.Http.Post;
            getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            getRequest.AllowWriteStreamBuffering = true;
            getRequest.ProtocolVersion = HttpVersion.Version11;
            getRequest.AllowAutoRedirect = true;
            getRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            getRequest.ContentLength = byteArray.Length;
            Stream newStream = getRequest.GetRequestStream(); //open connection
            newStream.Write(byteArray, 0, byteArray.Length); // Send the data.
            newStream.Close();

            HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
            using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        #endregion
    }
}