//////////////////////////////////////////////////////////////////////
// File Name    : RESTException
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/12/2015 11:23:42 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Net;

namespace Framework.Utility.Exception
{
    /// <summary>
    /// Restful exception
    /// </summary>
    public class RestException : System.Exception
    {
        /// <summary>
        /// <see cref="WebRequestMethods.Http"/> status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// </summary>
        public RestException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public RestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="httpStatusCode"></param>
        public RestException(string message, HttpStatusCode httpStatusCode)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}