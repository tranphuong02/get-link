//////////////////////////////////////////////////////////////////////
// File Name    : MailHelper
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 17/11/2015 4:02:08 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Framework.Utility
{
    /// <summary>
    /// Send email helper
    /// </summary>
    public partial class MailHelper
    {
        /// <summary>
        /// Time out
        /// </summary>
        private const int Timeout = 180000;

        /// <summary>
        /// Host
        /// <example><c>smtp</c>.<c>gmail</c>.com</example>
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// Port
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// User name of sender
        /// </summary>
        private readonly string _user;

        /// <summary>
        /// Password of sender
        /// </summary>
        private readonly string _pass;

        /// <summary>
        /// Use SSL or not
        /// </summary>
        private readonly bool _ssl;

        /// <summary>
        /// Sender
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Recipient
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// <c>Recipient</c> CC
        /// </summary>
        public string RecipientCc { get; set; }

        /// <summary>
        /// Subject of an email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Content of email
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// <c>Attachment</c> file in email
        /// </summary>
        public string AttachmentFile { get; set; }

        /// <summary>
        ///     Create a mail
        /// </summary>
        /// <param name="username">User name to login <paramref name="host"/></param>
        /// <param name="password">password to login host</param>
        /// <param name="ssl">default value: true</param>
        /// <param name="host">default value: smtp.gmail.com</param>
        /// <param name="port">default value: 587</param>
        public MailHelper(string username, string password, bool ssl = true, string host = "smtp.gmail.com", int port = 587)
        {
            //MailServer - Represents the SMTP Server
            _host = host;

            //Port- Represents the port number
            _port = port;

            //MailAuthUser and MailAuthPass - Used for Authentication for sending email
            _user = username;
            _pass = password;
            _ssl = ssl;
        }

        /// <summary>
        ///     Send email with configuration
        /// </summary>
        /// <exception cref="SmtpException">The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl" /> is set to true but the <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod" /> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory" /> or <see cref="F:System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis" />.-or-<see cref="P:System.Net.Mail.SmtpClient.EnableSsl" /> is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.</exception>
        /// <exception cref="SmtpFailedRecipientsException">The <paramref name="message" /> could not be delivered to one or more of the recipients in <see cref="P:System.Net.Mail.MailMessage.To" />, <see cref="P:System.Net.Mail.MailMessage.CC" />, or <see cref="P:System.Net.Mail.MailMessage.Bcc" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="message" /> is null.</exception>
        /// <exception cref="InvalidOperationException">This <see cref="T:System.Net.Mail.SmtpClient" /> has a <see cref="SmtpClient.SendAsync" /> call in progress.-or- <see cref="P:System.Net.Mail.MailMessage.From" /> is null.-or- There are no recipients specified in <see cref="P:System.Net.Mail.MailMessage.To" />, <see cref="P:System.Net.Mail.MailMessage.CC" />, and <see cref="P:System.Net.Mail.MailMessage.Bcc" /> properties.-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod" /> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network" /> and <see cref="P:System.Net.Mail.SmtpClient.Host" /> is null.-or-<see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod" /> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network" /> and <see cref="P:System.Net.Mail.SmtpClient.Host" /> is equal to the empty string ("").-or- <see cref="P:System.Net.Mail.SmtpClient.DeliveryMethod" /> property is set to <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network" /> and <see cref="P:System.Net.Mail.SmtpClient.Port" /> is zero, a negative number, or greater than 65,535.</exception>
        /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
        public void Send()
        {
            // We do not catch the error here... let it pass direct to the caller
            Attachment att = null;
            var message = new MailMessage(Sender, Recipient, Subject, Body) { IsBodyHtml = true };
            if (RecipientCc != null)
            {
                message.Bcc.Add(RecipientCc);
            }
            var smtp = new SmtpClient(_host, _port);

            if (!String.IsNullOrEmpty(AttachmentFile))
            {
                if (File.Exists(AttachmentFile))
                {
                    att = new Attachment(AttachmentFile);
                    message.Attachments.Add(att);
                }
            }

            if (_user.Length > 0 && _pass.Length > 0)
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_user, _pass);
                smtp.EnableSsl = _ssl;
            }

            smtp.Send(message);

            att?.Dispose();
            message.Dispose();
            smtp.Dispose();
        }

        public bool Send(IList<Byte[]> files, IList<string> fileNames, IList<string> mailCCs)
        {
            var message = new MailMessage(this.Sender, this.Recipient, this.Subject, this.Body) { IsBodyHtml = true };
            var smtp = new SmtpClient(this._host, this._port);

            // Add attachment
            if (files != null && fileNames != null)
            {
                for (var i = 0; i < files.Count; i++)
                {
                    if (fileNames[i] == null) continue;
                    var fileName = Path.GetFileName(fileNames[i]);
                    message.Attachments.Add(new Attachment(new MemoryStream(files[i]), fileName));
                }
            }

            if (mailCCs != null && mailCCs.Any())
            {
                foreach (var mailCC in mailCCs)
                    message.CC.Add(mailCC);
            }

            try
            {
                smtp.Send(message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(@"Exception caught in CreateTimeoutTestMessage(): {0}", ex);
            }
            finally
            {
                message.Dispose();
                smtp.Dispose();
            }

            return true;
        }
    }
}