using System;
using System.Globalization;

namespace Framework.Utility
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Get DateTime.UtcNow without milliseconds
        /// </summary>
        /// <returns></returns>
        public static DateTime GetUtcNowWithoutMillisecond()
        {
            DateTime utcNow = DateTime.UtcNow;
            return utcNow.TruncateTo(Enums.DateTruncate.Second);
        }

        /// <summary>
        /// truncate date time
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="truncateTo"></param>
        /// <returns></returns>
        public static DateTime TruncateTo(this DateTime dt, Enums.DateTruncate truncateTo)
        {
            if (truncateTo == Enums.DateTruncate.Year)
                return new DateTime(dt.Year, 0, 0);
            else if (truncateTo == Enums.DateTruncate.Month)
                return new DateTime(dt.Year, dt.Month, 0);
            else if (truncateTo == Enums.DateTruncate.Day)
                return new DateTime(dt.Year, dt.Month, dt.Day);
            else if (truncateTo == Enums.DateTruncate.Hour)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            else if (truncateTo == Enums.DateTruncate.Minute)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            else
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        public static DateTime? ParseDateTimeForSearch(string timeString, CultureInfo cultureInfo)
        {
            DateTime? keywordDate;
            try
            {
                keywordDate = DateTime.Parse(timeString, cultureInfo, DateTimeStyles.RoundtripKind);
            }
            catch
            {
                keywordDate = null;
            }
            return keywordDate;
        }

        public static DateTime? ParseDateTimeForSearch(string timeString)
        {
            DateTime? keywordDate;
            try
            {
                keywordDate = DateTime.Parse(timeString, new CultureInfo("vi-VN"), DateTimeStyles.RoundtripKind);
            }
            catch
            {
                keywordDate = null;
            }
            return keywordDate;
        }

        /// <summary>
        /// Combine date and time to full date time
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTimeFromDateAndTime(DateTime? date, string time)
        {
            try
            {
                if (!date.HasValue || string.IsNullOrEmpty(time) || !time.Contains(":"))
                {
                    return null;
                }

                var hours = int.Parse(time.Split(':')[0]);
                var minutes = int.Parse(time.Split(':')[1]);

                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hours, minutes, 0);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}