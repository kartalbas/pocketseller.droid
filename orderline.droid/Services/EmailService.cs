using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using Android.Content;
using Android.Text;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.droid.Services
{
    public class EmailService : core.Services.Interfaces.IMailService
    {
        private const int defaultPort = 587;
        private const string defaultTo = @"by@yilmazfeinkost.de";
        private const string defaultUsername = @"pocketsellersmtp@gmail.com";
        private const string defaultPassword = "VitaminY2021";
        private const string defaultSmtpServer = "smtp.gmail.com";
        private const string defaultImapServer = "imap.gmail.com";
        private const int defaultImapPort = 993;

        public bool CanSend => true;

        public void ShowDraft(string subject, string body, bool html, string[] to, string[] cc, string[] bcc, IEnumerable<string> attachments = null)
        {
            var intent = new Intent(Intent.ActionSendMultiple);

            intent.SetType(html ? "text/html" : "text/plain");
            intent.PutExtra(Intent.ExtraEmail, to);
            intent.PutExtra(Intent.ExtraCc, cc);
            intent.PutExtra(Intent.ExtraBcc, bcc);
            intent.PutExtra(Intent.ExtraSubject, subject ?? string.Empty);

            if (html)
            {
                intent.PutExtra(Intent.ExtraText, Html.FromHtml(body, FromHtmlOptions.ModeCompact));
            }
            else
            {
                intent.PutExtra(Intent.ExtraText, body ?? string.Empty);
            }

            if (attachments != null)
            {
                intent.AddAttachments(attachments);
            }

            this.StartActivity(intent);
        }

        public void ShowDraft(string subject, string body, bool html, string to, IEnumerable<string> attachments = null)
        {
            var intent = new Intent(Intent.ActionSendMultiple);
            intent.SetType(html ? "text/html" : "text/plain");
            intent.PutExtra(Intent.ExtraEmail, new[] { to });
            intent.PutExtra(Intent.ExtraSubject, subject ?? string.Empty);

            if (html)
            {
                intent.PutExtra(Intent.ExtraText, body);
            }
            else
            {
                intent.PutExtra(Intent.ExtraText, body ?? string.Empty);
            }

            intent.AddAttachments(attachments);

            this.StartActivity(intent);
        }

        public MailMessage CreateEmail(string from, string to, string subject, string body, List<string> files)
        {
            try
            {
                if (string.IsNullOrEmpty(from))
                    from = defaultUsername;

                if (string.IsNullOrEmpty(to))
                    to = defaultTo;

                if (string.IsNullOrEmpty(to))
                    throw new ArgumentNullException(nameof(to));

                if (string.IsNullOrEmpty(subject))
                    throw new ArgumentNullException(nameof(subject));

                if (string.IsNullOrEmpty(body))
                    throw new ArgumentNullException(nameof(body));

                var email = new MailMessage
                {
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(from),
                    To = { to },
                    Subject = subject,
                    Body = body
                };

                if (files?.Count > 0)
                {
                    foreach (var file in files)
                    {
                        email.Attachments.Add(new Attachment(file));
                    }
                }

                return email;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }
        }

        public void SendMail(MailMessage mailMessage, string smtpServer = null, int smtpPort = 0, string userName = null, string userPassword = null)
        {
            if (string.IsNullOrEmpty(smtpServer))
                smtpServer = defaultSmtpServer;

            if (smtpPort == 0)
                smtpPort = defaultPort;

            if (string.IsNullOrEmpty(userName))
                userName = defaultUsername;

            if (string.IsNullOrEmpty(userPassword))
                userPassword = defaultPassword;

            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = smtpServer,
                    Port = smtpPort,
                    EnableSsl = true,
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName, userPassword)
                };

                smtpClient.SendAsync(mailMessage, null);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }
        }

        public void DeleteMessage(UniqueId uid)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(defaultImapServer, defaultImapPort, true);
                    client.Authenticate(defaultUsername, defaultPassword);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);
                    inbox.AddFlags(uid, MessageFlags.Deleted, true);
                    inbox.Expunge();
                    client.Disconnect(true);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }
        }

        public Dictionary<UniqueId, MimeMessage> GetMails()
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(defaultImapServer, defaultImapPort, true);
                    client.Authenticate(defaultUsername, defaultPassword);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    var mailItems = inbox.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Size | MessageSummaryItems.Flags);

                    var messages = new Dictionary<UniqueId, MimeMessage>();

                    foreach (var mailItem in mailItems)
                    {
                        var message = inbox.GetMessage(mailItem.UniqueId);
                        messages.Add(mailItem.UniqueId, message);
                    }

                    client.Disconnect(true);

                    return messages;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }
        }
    }
}