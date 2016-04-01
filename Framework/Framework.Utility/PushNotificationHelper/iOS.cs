using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

namespace Framework.Utility.PushNotificationHelper
{
    /// <summary>
    /// Apple push notification
    /// </summary>
    public class iOS
    {
        /// <summary>
        /// Hex to data from hex string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] HexStringToByteArray(string hexString)
        {
            if (hexString == null)
                return null;

            if (hexString.Length % 2 == 1)
                hexString = '0' + hexString; // Up to you whether to pad the first or last byte

            byte[] data = new byte[hexString.Length / 2];

            for (int i = 0; i < data.Length; i++)
                data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return data;
        }

        public void SendListNotification(string certificationFullPath,
            string certificationPassword,
            bool isProduction,
            List<string> deviceTokens,
            object notificationMessage)
        {
            if (deviceTokens?.Any() != true)
            {
                return;
            }

            int port = 2195;
            var hostname = isProduction ? "gateway.push.apple.com" : "gateway.sandbox.push.apple.com";

            //load certificate
            X509Certificate2 clientCertificate = new X509Certificate2(certificationFullPath, certificationPassword);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);
            foreach (var deviceToken in deviceTokens)
            {
                TcpClient client = new TcpClient(hostname, port);
                SslStream sslStream = new SslStream(
                        client.GetStream(),
                        false,
                        ValidateServerCertificate,
                        null
                );

                try
                {
                    sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Default, true);
                }
                catch (AuthenticationException ex)
                {
                    Debug.WriteLine(ex);
                    client.Close();
                    return;
                }

                // use one of two
                SendRequest(notificationMessage, deviceToken, sslStream);
                //SendRequest(notificationMessage, deviceToken, sslStream, client);
            }
        }

        private void SendRequest(object notificationMessage, string deviceToken, SslStream sslStream)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            String payload = new JavaScriptSerializer().Serialize(notificationMessage);

            writer.Write((byte)2); // The command (version 2)

            // 4 bytes for the frameLength (this includes all the items ids listed below)
            int frameLength = 1 + 2 + 32 + 1 + 2 + Encoding.UTF8.GetByteCount(payload);
            // (tokenCommand + tokenLength + token) + (payloadCommand + payloadLength + payload)
            this.WriteIntBytesAsBigEndian(writer, frameLength, 4);

            // DEVICE ID
            writer.Write((byte)1); // Command for Item ID: deviceId
            byte[] tokenBytes = this.HexStringToByteArray(deviceToken);
            this.WriteIntBytesAsBigEndian(writer, tokenBytes.Length, 2);
            writer.Write(tokenBytes);

            // PAYLOAD
            writer.Write((byte)2); // Command for Item ID: payload
            this.WriteIntBytesAsBigEndian(writer, Encoding.UTF8.GetByteCount(payload), 2);

            byte[] value = Encoding.UTF8.GetBytes(payload);
            Debug.WriteLine(Encoding.UTF8.GetString(value));

            writer.Write(value, 0, Encoding.UTF8.GetByteCount(payload));

            writer.Flush();

            sslStream.Write(memoryStream.ToArray());
            sslStream.Flush();
        }

        private void SendRequest(object notificationMessage, string deviceToken, SslStream sslStream, TcpClient client)
        {
            // Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((byte)0); //The command
            writer.Write((byte)0); //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)

            writer.Write(HexStringToByteArray(deviceToken.ToUpper()));

            String payload = new JavaScriptSerializer().Serialize(notificationMessage);

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)Encoding.UTF8.GetByteCount(payload)); //payload length (big-endian second byte)

            byte[] b1 = Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);

            writer.Flush();

            byte[] array = memoryStream.ToArray();
            sslStream.Write(array);
            sslStream.Flush();

            // Close the client connection.
            client.Close();
        }

        private void WriteIntBytesAsBigEndian(BinaryWriter writer, int value, int bytesCount)
        {
            byte[] bytes = null;
            if (bytesCount == 2)
                bytes = BitConverter.GetBytes((Int16)value);
            else if (bytesCount == 4)
                bytes = BitConverter.GetBytes((Int32)value);
            else if (bytesCount == 8)
                bytes = BitConverter.GetBytes((Int64)value);

            if (bytes != null)
            {
                Array.Reverse(bytes);
                writer.Write(bytes, 0, bytesCount);
            }
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}