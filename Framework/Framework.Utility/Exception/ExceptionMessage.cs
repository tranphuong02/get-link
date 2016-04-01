//////////////////////////////////////////////////////////////////////
// File Name    : ExceptionMessage
// System Name  : Framework.Utility
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace Framework.Utility.Exception
{
    /// <summary>
    /// <see cref="Exception"/> message
    /// </summary>
    public static class ExceptionMessage
    {
        /// <summary>
        ///     Get exception message and write to log file
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="messageError"></param>
        /// <param name="sourceLineNumber"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="P:System.Text.StringBuilder.MaxCapacity" />. </exception>
        public static string GetExceptionMessage(this System.Exception ex, string messageError, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<Exception details>");
            strBuilder.AppendLine("    We have detected an exception. Message output start");
            strBuilder.AppendLine("    [Date and time of occurrence]");
            strBuilder.AppendLine("        " + DateTime.UtcNow.ToString(Constants.DateFormat.DateWithTime));
            strBuilder.AppendLine("    [Member Name]");
            strBuilder.AppendLine("        " + memberName);
            strBuilder.AppendLine("    [Source Path]");
            strBuilder.AppendLine("        " + sourceFilePath);
            strBuilder.AppendLine("    [Source Line Number]");
            strBuilder.AppendLine("        " + sourceLineNumber);
            strBuilder.AppendLine("    [Message content]");
            strBuilder.AppendLine("        " + messageError);
            strBuilder.AppendLine("    [Stack on the internal exception]");
            strBuilder.AppendLine("    [Exception class]      " + ex.GetType());
            strBuilder.AppendLine("    [Exception message]    " + ex.Message);
            strBuilder.AppendLine("    [Exception stack trace]" + ex.StackTrace);

            //Get InnerException Message
            if (ex.InnerException != null)
            {
                strBuilder.AppendLine();
                strBuilder.AppendLine("    [It outputs the exception that was detected by the internal.]");
                strBuilder.AppendLine("        [Exception class]       " + ex.InnerException.GetType());
                strBuilder.AppendLine("        [Exception message]     " + ex.InnerException.Message);
                strBuilder.AppendLine("        [Exception stack trace] " + ex.InnerException.StackTrace);
            }

            strBuilder.AppendLine("</Exception details>");
            strBuilder.AppendLine();

            //return exception info
            return strBuilder.ToString();
        }
    }
}