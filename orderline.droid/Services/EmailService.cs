using System.Collections.Generic;
using Android.Content;
using Android.Text;

namespace pocketseller.droid.Services
{
    public class EmailService : core.Services.Interfaces.IMailService
    {
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
    }
}