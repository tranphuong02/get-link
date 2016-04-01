//////////////////////////////////////////////////////////////////////
// File Name    : MD5Helper
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 06:16:38 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Security.Cryptography;
using System.Text;

namespace Framework.Utility
{
    /// <summary>
    /// <see cref="MD5"/> algorithm helper method
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        ///     Get md5 hash of string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="TargetInvocationException">The algorithm was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.</exception>
        public static string GetMd5Hash(this string value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return Hash(md5Hash, value);
            }
        }

        /// <summary>
        ///     Hash md5
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new String builder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            foreach (byte item in data)
            {
                sBuilder.Append(item.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}