using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Web.Helpers;

namespace Web.Controllers
{
    public class Getlink3Controller : Controller
    {
        public ActionResult Index()
        {
            HttpWebRequest request;
            HttpWebResponse response;
            CookieContainer cookies;
            string html;
            string csrf;
            string postData;

            // csrf
            request = (HttpWebRequest)WebRequest.Create(Configs.DownloadLink);
            request.AllowAutoRedirect = false;
            request.CookieContainer = new CookieContainer();
            response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                html = stream.ReadToEnd();
            }
            csrf = GetLinkHelper.FsLoginCsrfName(GetLinkHelper.HtmlDocument(html), Configs.FsCsrfName);
            response.Close();

            // Login
            postData = string.Format(Configs.RequestLoginParams, csrf);
            request = (HttpWebRequest)WebRequest.Create(Configs.LoginUrl);
            request.AllowAutoRedirect = false;
            request.Method = WebRequestMethods.Http.Post;
            request.CookieContainer = cookies;
            

            // Headers
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
            //request.Headers.Add("Accept-Encoding", "gzip");

            // Content length
            var byteArray = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                html = stream.ReadToEnd();
            }
            response.Close();

            // Reload download file page
            request = (HttpWebRequest)WebRequest.Create(Configs.DownloadLink);
            request.AllowAutoRedirect = false;
            request.CookieContainer = cookies;
            response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                html = stream.ReadToEnd();
            }
            csrf = GetLinkHelper.FsLoginCsrfName(GetLinkHelper.HtmlDocument(html), Configs.FsCsrfName);
            response.Close();

            // Download file
            postData = string.Format(Configs.RequestDownloadParams, csrf);
            request = (HttpWebRequest)WebRequest.Create(Configs.GetFileLink);
            request.AllowAutoRedirect = false;
            request.Method = WebRequestMethods.Http.Post;
            request.CookieContainer = cookies;

            // Headers
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
            //request.Headers.Add("Accept-Encoding", "gzip");

            // Content length
            byteArray = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            newStream = request.GetRequestStream();

            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                html = stream.ReadToEnd();
            }
            response.Close();


            return View();
        }
    }
}