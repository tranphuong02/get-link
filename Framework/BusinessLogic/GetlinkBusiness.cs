//////////////////////////////////////////////////////////////////////
// File Name    : FShareBusiness
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 3/31/2016 3:38:16 PM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.Logger.Log4Net;
using Framework.Utility;
using HtmlAgilityPack;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Transverse.Enums;
using Transverse.Interfaces.Business;
using Transverse.Interfaces.DAL;
using Transverse.Models.Business.Getlink;
using Transverse.Models.DAL;
using Transverse.Utils;
using BaseModel = Transverse.Models.Business.BaseModel;
using Constants = Transverse.Constants;

namespace BusinessLogic
{
    public class GetlinkBusiness : IGetlinkBusiness
    {
        [Dependency]
        public IRequestRepository RequestRepository { get; set; }

        [Dependency]
        public ICustomerRepository CustomerRepository { get; set; }

        [Dependency]
        public IBlackListRepository BlackListRepository { get; set; }

        [Dependency]
        public IDbContext DbContext { get; set; }

        public HtmlDocument HtmlDocument(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public string GetFsCsrf(HtmlDocument document)
        {
            var input = document.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == Constants.FShare.FsCsrfName));
            return input == null ? "" : input.Attributes["value"].Value;
        }

        private string GetLinkCode(HtmlDocument document)
        {
            var input = document.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == Constants.FShare.LinkCodeName));
            return input == null ? "" : input.Attributes["value"].Value;
        }

        public BaseModel Getlink(GetlinkParamViewModel viewModel)
        {
            try
            {
                // Validate the limited of ip address
                var getable = IsGetable(viewModel);

                if (getable.CanGet == false)
                {
                    return new BaseModel
                    {
                        IsSuccess = false,
                        ErrorCode = (int)HttpStatusCode.BadRequest,
                        Message = "Limited free request"
                    };
                }
                // Init get link type
                InitGetLinkType(viewModel);

                switch (viewModel.Type)
                {
                    case (int)GetlinkType.FShare:
                        {
                            var data = GetFShareLink(viewModel.Url, viewModel.Password, getable.TotalRequestToday, getable.IsRequiredAds);
                            if (data != null)
                            {
                                return new BaseModel
                                {
                                    IsSuccess = true,
                                    ErrorCode = (int) HttpStatusCode.OK,
                                    Data = data
                                };
                            }
                            return new BaseModel
                            {
                                IsSuccess = false,
                                ErrorCode = (int)HttpStatusCode.BadRequest,
                                Message = "Không kết nối được tới host, bạn vui lòng thử lại sau"
                            };
                        }
                }
            }
            catch (Exception ex)
            {
                Provider.Instance.LogError(ex);
            }

            return new BaseModel
            {
                IsSuccess = false,
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = "Không kết nối được tới host, bạn vui lòng thử lại sau"
            };
        }

        public FShareResultViewModel GetFShareLink(string url, string password, int totalRequestToday, bool isRequiredAds)
        {
            var cookieContainer = new CookieContainer();
            var linkCode = string.Empty;
            var htmlDocument = new HtmlDocument();

            // Login
            var csrf = GetFsCsrf(url, ref cookieContainer, ref htmlDocument, ref linkCode);
            Login(csrf, ref cookieContainer);

            // Download
            csrf = GetFsCsrf(url, ref cookieContainer, ref htmlDocument, ref linkCode);
            var result = DownloadFile(csrf, linkCode, cookieContainer);

            if (result.url == null)
            {
                return null;
            }

            RewriteUrl(result);

            #region Insert request

            var userRequest = new Request
               {
                   Ip = HttpContext.Current.Request.UserHostAddress,
                   RequestUrl = url,
                   RequestPassword = password,
                   RequestType = (int)GetlinkType.FShare,
                   ResultUrl = result.url,
                   ResultName = "",
                   ResultSize = "",
                   IsDelete = false,
                   CreatedDate = DateTimeHelper.UtcNow()
               };
            GenerateFileInformation(userRequest, htmlDocument);
            RequestRepository.Insert(userRequest);
            DbContext.SaveChanges();

            #endregion Insert request

            #region GenerateAds

            result.is_required_ads = isRequiredAds;
            if (isRequiredAds)
            {
                GenerateAds(result, totalRequestToday, userRequest.Id);
            }

            #endregion GenerateAds

            #region Update request

            userRequest.ResultAdsUrl = result.ads_url;
            userRequest.ResultAdsType = result.ads_type;
            userRequest.IsActive = true;
            userRequest.ModifiedDate = DateTimeHelper.UtcNow();
            RequestRepository.Update(userRequest);
            DbContext.SaveChanges();

            #endregion

            result.file_name = userRequest.ResultName;
            result.file_size = userRequest.ResultSize;
            result.url = "";

            return result;
        }

        #region Helper Methods

        private string GetFsCsrf(string url, ref CookieContainer cookies, ref HtmlDocument document, ref string linkCode)
        {
            string html;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookies;

            var response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;

            var responseStream = response.GetResponseStream();
            if (responseStream == null) return "";

            using (var stream = new StreamReader(responseStream))
            {
                html = stream.ReadToEnd();
            }
            response.Close();

            document = HtmlDocument(html);
            linkCode = GetLinkCode(document);
            return GetFsCsrf(document);
        }

        private void Login(string csrf, ref CookieContainer cookies)
        {
            var postData = string.Format(Constants.FShare.RequestLoginParams, csrf);
            var request = InitPost(cookies, Constants.FShare.LoginUrl, postData);
            var response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            response.Close();
        }

        private FShareResultViewModel DownloadFile(string csrf, string linkCode, CookieContainer cookies)
        {
            var postData = string.Format(Constants.FShare.RequestDownloadParams, csrf, linkCode);
            var request = InitPost(cookies, Constants.FShare.GetFileUrl, postData);
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) return null;
            using (var stream = new StreamReader(responseStream))
            {
                var fshareResult = stream.ReadToEnd();
                response.Close();
                return JsonConvert.DeserializeObject<FShareResultViewModel>(fshareResult);
            }
        }

        private HttpWebRequest InitPost(CookieContainer cookies, string url, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;
            request.Method = WebRequestMethods.Http.Post;
            request.CookieContainer = cookies;

            // Headers
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";

            // Content length
            var byteArray = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            return request;
        }

        private GetableViewModel IsGetable(GetlinkParamViewModel viewModel)
        {
            try
            {
                var ipAddress = HttpContext.Current.Request.UserHostAddress;
                var now = DateTimeHelper.UtcNow();

                if (string.IsNullOrWhiteSpace(ipAddress)
                    || BlackListRepository.IsInBlackList(ipAddress))
                {
                    return new GetableViewModel
                    {
                        CanGet = false
                    };
                }

                if (viewModel.CustomerId != null && viewModel.CustomerType != (int)CustomerType.Normal)
                {
                    return new GetableViewModel
                   {
                       CanGet = true,
                       IsRequiredAds = false
                   };
                }

                if (viewModel.CustomerId != null)
                {
                    var totalRequestToday = RequestRepository.TotalCustomerRequest(viewModel.CustomerId.Value, now);
                    return new GetableViewModel
                    {
                        CanGet = totalRequestToday <= BackendHelpers.FreeRequestNumber,
                        TotalRequestToday = totalRequestToday,
                        IsRequiredAds = true
                    };
                }
                else
                {
                    var totalRequestToday = RequestRepository.TotalIpRequest(ipAddress, now);
                    return new GetableViewModel
                   {
                       CanGet = totalRequestToday <= BackendHelpers.FreeRequestNumber,
                       TotalRequestToday = totalRequestToday,
                       IsRequiredAds = true
                   };
                }
            }
            catch (Exception ex)
            {
                Provider.Instance.LogError(ex);
                return new GetableViewModel
                {
                    CanGet = false
                };
            }
        }

        private void InitGetLinkType(GetlinkParamViewModel viewModel)
        {
            if (viewModel.Url.Contains("fshare.vn"))
            {
                viewModel.Type = (int) GetlinkType.FShare;
                return;
            }
        }

        private void RewriteUrl(FShareResultViewModel viewModel)
        {
            try
            {
                viewModel.url = viewModel.url.Replace(@"\/", "");
                var insertPotition = viewModel.url.LastIndexOf("/", StringComparison.Ordinal) + 1;
                viewModel.url = HttpUtility.UrlDecode(viewModel.url.Insert(insertPotition, string.Format("[{0}]-", Constants.AppName)));
            }
            catch (Exception ex)
            {
                Provider.Instance.LogError(ex);
            }
        }

        private void GenerateAds(FShareResultViewModel viewModel, int totalRequestToday, int id)
        {
            try
            {
                var downloadLink = BackendHelpers.RootUrl + "dl/" + id;
                // ouo
                if (totalRequestToday < 3)
                {
                    viewModel.ads_type = (int)AdsType.Ouo;
                    viewModel.ads_introduction = BackendHelpers.OuoIntroduction;
                    GenerateOuoAds(viewModel, downloadLink);
                }
                // adf ly
                else //if (totalRequestToday < 6)
                {
                    viewModel.ads_type = (int)AdsType.Adf;
                    viewModel.ads_introduction = BackendHelpers.AdfIntroduction;
                    GenerateAdfAds(viewModel, downloadLink);
                }
            }
            catch (Exception ex)
            {
                Provider.Instance.LogError(ex);
            }
        }

        private void GenerateFileInformation(Request request, HtmlDocument htmlDocument)
        {
            try
            {
                var fileInfo = htmlDocument.DocumentNode.Descendants().FirstOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "file-info");
                if (fileInfo != null && fileInfo.ChildNodes != null && fileInfo.ChildNodes.Any())
                {
                    request.ResultName = fileInfo.ChildNodes.ElementAt(1).InnerText.Trim();
                    request.ResultSize = fileInfo.ChildNodes.ElementAt(3).InnerText.Trim().Replace("\r\n", "").Replace(@"Nhấn vào để thêm link yêu thích", "");
                }
            }
            catch (Exception ex)
            {
                Provider.Instance.LogError(ex);
            }
        }

        private void GenerateOuoAds(FShareResultViewModel viewModel, string downloadUrl)
        {
            var adsUrl = BackendHelpers.OuoUrl + downloadUrl;
            var request = (HttpWebRequest)WebRequest.Create(adsUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) return;

            using (var stream = new StreamReader(responseStream))
            {
                var htmlDocument = HtmlDocument(stream.ReadToEnd());

                viewModel.ads_url = htmlDocument.DocumentNode.InnerText;
            }
            response.Close();
        }

        private void GenerateAdfAds(FShareResultViewModel viewModel, string downloadUrl)
        {
            var adsUrl = BackendHelpers.AdfUrl + downloadUrl;
             var request = (HttpWebRequest)WebRequest.Create(adsUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) return;

            using (var stream = new StreamReader(responseStream))
            {
                var htmlDocument = HtmlDocument(stream.ReadToEnd());

                viewModel.ads_url = htmlDocument.DocumentNode.InnerText;
            }
            response.Close();
        }

        #endregion Helper Methods
    }
}