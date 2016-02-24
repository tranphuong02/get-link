using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Web.Helpers;

namespace Web.Controllers
{
    public class Getlink2Controller : Controller
    {
        public ActionResult Index()
        {
            Download();
            return View();
        }

        private void Download()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
                webClient.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

                // Login
                var fileHtml = webClient.DownloadFilePage();
                var fileDocument = fileHtml.HtmlDocument();
                var fsLoginCsrf = fileDocument.FsLoginCsrfName(Configs.FsCsrfName);
                var loginParams = new NameValueCollection
                {
                    {"LoginForm[email]", "tran.phuongvd02@gmail.com"},
                    {"LoginForm[password]", "trUng zjn 12#"},
                    {"LoginForm[rememberMe]", "0"},
                    {"LoginForm[rememberMe]", "1"},
                    {"fs_csrf", fsLoginCsrf},
                    {"yt0", "Đăng nhập"}
                };
                
                //var loginUpload = webClient.UploadValues(Configs.LoginUrl, "POST", loginParams);
                //var loginResult = Encoding.UTF8.GetString(loginUpload);



            }
        }
    }
}