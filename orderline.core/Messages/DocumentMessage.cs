using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EDocumentAction
    {
        ShowSelectionArticle,
        ShowSelectionAddress,
        ShowDocumentDetail,
        ShowDocumentOrder,
        InitDocumentOrder,
        ShowAddress
    }

    public enum ActionMode
    {
        Order,
        Quotation
    }

    public class DocumentMessage : MvxMessage
    {
        public DocumentMessage(object sender, EDocumentAction EDocumentAction, ActionMode actionMode = ActionMode.Order)
            : base(sender)
        {
            this.ActionMode = actionMode;
            this.EDocumentAction = EDocumentAction;
        }

        public ActionMode ActionMode { get; set; }

        public EDocumentAction EDocumentAction { get; set; }
    }
}
