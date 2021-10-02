using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum ETitleAction
    {
        Changed
    }

    public class TitleMessage : MvxMessage
    {
        public TitleMessage(object sender, ETitleAction ETitleAction)
            : base(sender)
        {
            this.ETitleAction = ETitleAction;
        }

        public ETitleAction ETitleAction { get; set; }
    }
}
