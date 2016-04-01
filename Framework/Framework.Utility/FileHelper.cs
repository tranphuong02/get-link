using System;
using System.IO;
using System.Text;

namespace Framework.Utility
{
    public static class FileHelper
    {
        public static void ByteArrayToFile(this Byte[] byteArray, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Open file for reading
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            // Writes a block of bytes to this stream using data from
            // a byte array.
            fileStream.Write(byteArray, 0, byteArray.Length);

            // close file stream
            fileStream.Close();
        }

        public static void StringBuilderToFile(this StringWriter stringWriter, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            byte[] buffer = Encoding.ASCII.GetBytes(stringWriter.ToString());

            // Open file for reading
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            // Writes a block of bytes to this stream using data from
            // a byte array.
            fileStream.Write(buffer, 0, buffer.Length);

            // close file stream
            fileStream.Close();
        }
    }
}