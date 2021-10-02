using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EDocumentServiceAction
    {
        AddDocumentdetail,
        RemoveDocumentdetail,
        UpdateDocumentdetail,
        SaveDocument,
        UpdateDocument,
        DeleteDocument,
        DiscardDocument
    }

    public class DocumentServiceMessage : MvxMessage
    {
        public DocumentServiceMessage(object sender, EDocumentServiceAction enmEDocumentServiceAction)
            : base(sender)
        {
            EDocumentServiceAction = enmEDocumentServiceAction;
        }

        public EDocumentServiceAction EDocumentServiceAction { get; set; }
    }
}
