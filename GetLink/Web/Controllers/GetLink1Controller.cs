using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using HtmlAgilityPack;
using Web.Helpers;

namespace Web.Controllers
{
    public class GetLink1Controller : Controller
    {
        public ActionResult Index()
        {//
           // GetLink();

            var web = new WebClient();
            web.DownloadString(Configs.DownloadLink);
            //web.UploadValues(Configs.DownloadLink);

            return View();
        }

        private void GetLink()
        {
            var cookies = new CookieContainer();
            GetLinkHelper.Get(Configs.LogoutUrl, ref cookies);

            // Login
            var loginHtml = GetLinkHelper.Get(Configs.DownloadLink, ref cookies);
            var loginFsCsrf = GetLinkHelper.FsLoginCsrfName(GetLinkHelper.HtmlDocument(loginHtml), Configs.FsCsrfName);
            var postLoginData = string.Format(Configs.RequestLoginParams, loginFsCsrf);
            var downloadHtml = GetLinkHelper.Post(Configs.LoginUrl, postLoginData, ref cookies);

            // Post download link
            var fileFsCsrf = GetLinkHelper.FsDownloadCsrfName(GetLinkHelper.HtmlDocument(downloadHtml), Configs.FsCsrfName);
            var postDownloadData = string.Format(Configs.RequestDownloadParams, fileFsCsrf);
            GetLinkHelper.Post(Configs.GetFileLink, postDownloadData, ref cookies);

            // Logout
            GetLinkHelper.Get(Configs.LogoutUrl, ref cookies);
        }
    }

    
}