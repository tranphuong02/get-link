//////////////////////////////////////////////////////////////////////
// File Name    : Constants
// System Name  : Framework.Utility
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Framework.Utility
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Date time format
        /// </summary>
        public static class DateFormat
        {
            /// <summary>
            ///     yyyyMMddHHmmss
            /// </summary>
            public const string yyyyMMddHHmmss = "yyyyMMddHHmmss";

            /// <summary>
            ///      yyyy/MM/dd HH:mm:ss
            /// </summary>
            public const string DateWithTime = "yyyy/MM/dd HH:mm:ss";

            /// <summary>
            ///     yyyy/MM/dd
            /// </summary>
            public const string DateOnly = "yyyy/MM/dd";

            public const string ddMMyyyy = "dd/MM/yyyy";
        }

        /// <summary>
        /// Status message
        /// </summary>
        public static class StatusMessage
        {
            /// <summary>
            /// Define a key for status message text
            /// </summary>
            public const string StatusMessageText = "StatusMessageText";

            /// <summary>
            /// Define a key for status message type
            /// </summary>
            public const string StatusMessageType = "StatusMessageType";
        }
    }
}