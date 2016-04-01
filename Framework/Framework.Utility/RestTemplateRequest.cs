//////////////////////////////////////////////////////////////////////
// File Name    : RestTemplateRequest
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/12/2015 11:21:45 AM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.Utility.Exception;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Framework.Utility
{
    /// <summary>
    /// Rest template for request
    /// </summary>
    public class RestTemplateRequest
    {
        /// <summary>
        ///     <see cref="WebRequestMethods.Http"/> client send get request
        /// </summary>
        /// <typeparam name="T"><see langword="object"/> type</typeparam>
        /// <param name="url">request url</param>
        /// <param name="requestParams">request param</param>
        /// <param name="headers">additional header</param>
        /// <returns></returns>
        /// <exception cref="RestException">Content is null.</exception>
        public static async Task<T> Get<T>
            (string url, IDictionary<string, string> requestParams = null, IDictionary<string, string> headers = null)
            where T : class
        {
            //Build query string
            url = BuildQuery(url, requestParams);

            //add customer header
            using (HttpRequestMessage requestMessage = BuildHeader(url, headers, HttpMethod.Get))
            {
                using (var client = new HttpClient(new HttpClientHandler()))
                {
                    //send request
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content == null)
                        {
                            throw new RestException(HttpStatusCode.InternalServerError.ToString(), HttpStatusCode.InternalServerError);
                        }
                        string result = await response.Content.ReadAsStringAsync();
                        if (result != null)
                        {
                            // parsing response data
                            var objects = new JavaScriptSerializer().Deserialize<T>(result);
                            return objects;
                        }
                    }
                    else
                    {
                        throw new RestException(response.ReasonPhrase, response.StatusCode);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// POST request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <param name="headers"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public static async Task<T> Post<T>
            (string url, string jsonData = "", IDictionary<string, string> headers = null) where T : class
        {
            using (HttpRequestMessage requestMessage = BuildHeader(url, headers, HttpMethod.Post))
            {
                requestMessage.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient(new HttpClientHandler()))
                {
                    //send request

                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new RestException(response.ReasonPhrase, response.StatusCode);
                    }
                    if (response.Content == null)
                    {
                        throw new RestException(HttpStatusCode.InternalServerError.ToString(), HttpStatusCode.InternalServerError);
                    }
                    string result = await response.Content.ReadAsStringAsync();
                    if (result == null)
                    {
                        return null;
                    }
                    // parsing response data
                    var objects = new JavaScriptSerializer().Deserialize<T>(result);
                    return objects;
                }
            }
        }

        #region Private helper

        /// <summary>
        /// Create query
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        private static string BuildQuery(string url, IEnumerable<KeyValuePair<string, string>> requestParams)
        {
            var builder = new UriBuilder(url);
            NameValueCollection queryBuilder = HttpUtility.ParseQueryString(builder.Query);
            if (requestParams != null)
            {
                foreach (var param in requestParams)
                {
                    queryBuilder[param.Key] = param.Value;
                }
            }
            builder.Query = queryBuilder.ToString();
            return builder.ToString();
        }

        /// <summary>
        /// Helper to build query
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static HttpRequestMessage BuildHeader(string url, IEnumerable<KeyValuePair<string, string>> headers, HttpMethod method)
        {
            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }
            return requestMessage;
        }

        #endregion Private helper
    }
}