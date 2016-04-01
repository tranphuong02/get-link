namespace Transverse.Models.Business
{
    public class BaseModel
    {
        /// <summary>
        /// Response code of API follow Restful template
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Error code if <see cref="IsSuccess"/> is false
        /// <remarks>Follow Rest API response code</remarks>
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Data of API
        /// </summary>
        public object Data { get; set; }
    }
}