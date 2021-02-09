using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum ESourceViewAction
    {
        Added,
        Updated,
        Deleted
    }

    public class SourceViewMessage : MvxMessage
    {
        public SourceViewMessage(object sender, ESourceViewAction enmSourceViewAction)
            : base(sender)
        {
            ESourceViewAction = enmSourceViewAction;
        }

        public ESourceViewAction ESourceViewAction { get; set; }
    }
}
