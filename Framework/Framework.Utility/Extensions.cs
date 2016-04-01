//////////////////////////////////////////////////////////////////////
// File Name    : Extensions
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 10:12:46 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UnidecodeSharpFork;

namespace Framework.Utility
{
    /// <summary>
    /// Extension
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Safe set value to dictionary
        /// </summary>
        /// <remarks>Update value if already exists, else is add</remarks>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
        /// <exception cref="NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        public static void SafeSetValue<T1, T2>(this IDictionary<T1, T2> source, T1 key, T2 data) where T1 : class
        {
            if (source.ContainsKey(key))
            {
                source[key] = data;
            }
            else
            {
                source.Add(key, data);
            }
        }

        /// <summary>
        /// Remove all entity html
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHTMLEntity(this string input)
        {
            //return Regex.Replace(input, "<.*?>", String.Empty);
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// Remove all tag html
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHTMLTag(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /// <summary>
        /// Change time of datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static string WithoutRootPath(this string value)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            return value?.Replace(url.Content("~"), @"/");
        }

        /// <summary>
        /// Remove all diacritics (accents) in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string value)
        {
            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Remove all diacritics (accents) in string and uni-decode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveDiacriticsAndUnidecode(this string value)
        {
            var normalizedString = value.Unidecode().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Check string is url or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUrl(this string value)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(value, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }
    }
}