using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EDocumentdetailAction
    {
        Stay,
        Exit
    }

    public class DocumentdetailMessage : MvxMessage
    {
        public DocumentdetailMessage(object sender, EDocumentdetailAction enmEDocumentFlowAction)
            : base(sender)
        {
            EDocumentdetailAction = enmEDocumentFlowAction;
        }

        public EDocumentdetailAction EDocumentdetailAction { get; set; }
    }
}
