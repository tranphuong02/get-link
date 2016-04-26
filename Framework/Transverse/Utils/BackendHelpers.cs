//////////////////////////////////////////////////////////////////////
// File Name    : BackendHelpers
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 4/1/2016 11:36:21 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Configuration;

namespace Transverse.Utils
{
    public static class BackendHelpers
    {
        public static int FreeRequestNumber => int.Parse(ConfigurationManager.AppSettings["MaximumDownloadTurn"]);
        public static string RootUrl => ConfigurationManager.AppSettings["RootUrl"];
        public static string OuoUrl => ConfigurationManager.AppSettings["OuoUrl"];
        public static string BcUrl => ConfigurationManager.AppSettings["BcUrl"];
        public static string LShrinkUrl => ConfigurationManager.AppSettings["LShrinkUrl"];
        public static string AdfUrl => ConfigurationManager.AppSettings["AdfUrl"];
    }
}