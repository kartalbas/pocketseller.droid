using System.Collections.Generic;

namespace pocketseller.core.Services.Interfaces
{
    public interface IMailService
    {
        bool CanSend { get; }

        void ShowDraft(string subject, string body, bool html, string to, IEnumerable<string> attachments = null);

        void ShowDraft(string subject, string body, bool html, string[] to, string[] cc, string[] bcc, IEnumerable<string> attachments = null);
    }
}