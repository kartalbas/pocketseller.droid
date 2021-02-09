using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EDocumentsViewAction
    {
        Added,
        Updated,
        Deleted,
        SourceChanged
    }

    public class DocumentsViewServiceMessage : MvxMessage
    {
        public DocumentsViewServiceMessage(object sender, EDocumentsViewAction enmDocumentsViewAction)
            : base(sender)
        {
            EDocumentsViewAction = enmDocumentsViewAction;
        }

        public EDocumentsViewAction EDocumentsViewAction { get; set; }
    }
}
