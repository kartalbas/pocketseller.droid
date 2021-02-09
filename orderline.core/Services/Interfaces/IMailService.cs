using System.Collections.Generic;
using System.Net.Mail;
using MailKit;
using MimeKit;

namespace pocketseller.core.Services.Interfaces
{
    public interface IMailService
    {
        MailMessage CreateEmail(string from, string to, string subject, string body, List<string> files);

        void SendMail(MailMessage mailMessage, string smtpServer = null, int smtpPort = 0, string userName = null, string userPassword = null);

        void DeleteMessage(UniqueId uid);

        Dictionary<UniqueId, MimeMessage> GetMails();

        bool CanSend { get; }

        void ShowDraft(string subject, string body, bool html, string to, IEnumerable<string> attachments = null);

        void ShowDraft(string subject, string body, bool html, string[] to, string[] cc, string[] bcc, IEnumerable<string> attachments = null);
    }
}