namespace Web.Helpers
{
    public static class Configs
    {
        public static string LoginUrl = "https://www.fshare.vn/login";
        public static string LogoutUrl = "https://www.fshare.vn/logout";
        public static string GetFileLink = "https://www.fshare.vn/download/get";
        public static string DownloadLink = "https://www.fshare.vn/file/T0R2T7630T/";
        public static string FsCsrfName = "fs_csrf";
        public static string LinkCodeName = "DownloadForm[linkcode]";
        public static string RequestLoginParams = "fs_csrf={0}&LoginForm%5Bemail%5D=tran.phuongvd02%40gmail.com&LoginForm%5Bpassword%5D=trUng+zjn+12%23&LoginForm%5Bcheckloginpopup%5D=0&LoginForm%5BrememberMe%5D=0&yt0=%C4%90%C4%83ng+nh%E1%BA%ADp";
        public static string RequestDownloadParams = "fs_csrf={0}&DownloadForm%5Bpwd%5D=&DownloadForm%5Blinkcode%5D=T0R2T7630T&ajax=download-form&undefined=undefined";
    }
}