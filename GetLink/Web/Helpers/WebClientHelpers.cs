//////////////////////////////////////////////////////////////////////
// File Name    : WebClientHelpers
// Summary      :
// Author       : phuong.tran
// Change Log   : 2/24/2016 5:56:26 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Net;

namespace Web.Helpers
{
    public static class WebClientHelpers
    {
        public static string DownloadFilePage(this WebClient webClient)
        {
            return webClient.DownloadString(Configs.DownloadLink);
        }
    }
}