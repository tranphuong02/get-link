//////////////////////////////////////////////////////////////////////
// File Name    : Serialize
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 6:23:34 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Framework.Utility
{
    /// <summary>
    /// Serialize helper
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        ///     To serialize <see langword="object"/> to string XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" />
        /// or <paramref name="encoding" /> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="stream" /> is
        /// not writable. </exception>
        /// <exception cref="OverflowException"><paramref name="value" /> is
        /// greater than <see cref="F:System.Int32.MaxValue" /> or less than
        /// <see cref="F:System.Int32.MinValue" />. </exception>
        public static string ToXmlString<T>(T objectToSerialize)
        {
            using (var stream = new MemoryStream())
            {
                TextWriter writer = new StreamWriter(stream, new UTF8Encoding());

                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(writer, objectToSerialize);
                return Encoding.UTF8.GetString(stream.ToArray(), 0, Convert.ToInt32(stream.Length));
            }
        }

        /// <summary>
        ///     To serialize string XML to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">The specified file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException">Part of the filename or directory cannot be found.</exception>
        public static T FromXmlString<T>(string source)
        {
            using (var sr = new XmlTextReader(source))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// To serialize <see langword="object"/> to stream
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        /// <summary>
        /// To serializeDeserialize <paramref name="stream"/> to object
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="IOException">Seeking is attempted before the
        /// beginning of the stream. </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset" /> is greater than
        /// <see cref="F:System.Int32.MaxValue" />. </exception>
        /// <exception cref="ArgumentException">There is an invalid
        /// <see cref="T:System.IO.SeekOrigin" />. -or-
        /// <paramref name="offset" /> caused an arithmetic overflow.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The current stream
        /// instance is closed. </exception>
        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }
}