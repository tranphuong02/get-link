//////////////////////////////////////////////////////////////////////
// File Name    : XmlHelper
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 6:24:10 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.IO;

namespace Framework.Utility
{
    /// <summary>
    /// Xml helper
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// Update backup <paramref name="data"/> <see langword="object"/> <see langword="override"/> to xml file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void UpdateConfigure(dynamic data, string path)
        {
            using (var sw = new StreamWriter(path, false))
            {
                sw.Write(Serialize.ToXmlString(data));
            }
        }

        /// <summary>
        /// Get xml <see langword="object"/> by <paramref name="path"/>
        /// </summary>
        /// <returns></returns>
        public static T GetConfigure<T>(string path)
        {
            return Serialize.FromXmlString<T>(path);
        }
    }
}