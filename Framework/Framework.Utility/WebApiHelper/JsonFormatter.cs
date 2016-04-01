using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;

namespace Framework.Utility.WebApiHelper
{
    /// <summary>
    /// Format json when response of Web API
    /// </summary>
    public static class JsonFormatter
    {
        /// <summary>
        /// Format json to camel case
        /// </summary>
        /// <param name="formatters"></param>
        public static void CamelCasePropertyNamesContractResolver(MediaTypeFormatterCollection formatters)
        {
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            //settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}