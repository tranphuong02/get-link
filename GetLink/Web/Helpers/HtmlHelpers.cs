//////////////////////////////////////////////////////////////////////
// File Name    : HtmlHelpers
// Summary      :
// Author       : phuong.tran
// Change Log   : 2/24/2016 5:58:42 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Linq;
using HtmlAgilityPack;

namespace Web.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlDocument HtmlDocument(this string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static string FsLoginCsrfName(this HtmlDocument html, string inputName)
        {
            var input = html.DocumentNode.Descendants("input").FirstOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == inputName));

            return input == null ? "" : input.Attributes["value"].Value;
        }

        public static string FsDownloadCsrfName(this HtmlDocument html, string inputName)
        {
            var input = html.DocumentNode.Descendants("input").LastOrDefault(x => x.Attributes != null && x.Attributes.Any(y => y.Value == inputName));

            return input == null ? "" : input.Attributes["value"].Value;
        }
    }
}