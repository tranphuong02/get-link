using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Framework.Utility.PushNotificationHelper
{
    /// <summary>
    /// Android - Google clound message
    /// </summary>
    public class Android
    {
        public class PostData
        {
            public PostData()
            {
                registration_ids = new List<string>();
            }

            public dynamic data { get; set; }
            public List<string> registration_ids { get; set; }
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="deviceRegIds">list device register id maximum is 1000</param>
        /// <param name="notificationMessage"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public void SendListNotification(string appId, List<string> deviceRegIds, object notificationMessage)
        {
            IEnumerable<List<string>> temp = deviceRegIds.Chunk(1000);
            List<List<string>> listToSend = temp.ToList();
            foreach (var list in listToSend)
            {
                SendNotification(appId, list, notificationMessage);
            }
        }

        private void SendNotification(string appId, List<string> deviceRegIds, object notificationMessage)
        {
            WebRequest tRequest = null;
            GetWebRequestAndroidCloud(ref tRequest, appId, "application/json");

            PostData postData = new PostData
            {
                data = notificationMessage,
                registration_ids = deviceRegIds
            };

            string postDataJson = new JavaScriptSerializer().Serialize(postData);

            GetResponse(tRequest, postDataJson);
        }

        #region Private Helper Methods

        private static void GetResponse(WebRequest tRequest, string postData)
        {
            using (var streamWriter = new StreamWriter(tRequest.GetRequestStream()))
            {
                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)tRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Debug.WriteLine(result, "Information-SendNotification-Android");
                }
            }
        }

        private static void GetWebRequestAndroidCloud(ref WebRequest tRequest, string appId, string contentType)
        {
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = contentType;
            tRequest.Headers.Add($"Authorization: key={appId}");
        }

        #endregion Private Helper Methods
    }
}