using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EWorkingAction
    {
        ShowWorking,
        HideWorking
    }

    public class WorkingMessage : MvxMessage
    {
        public WorkingMessage(object sender, EWorkingAction EWorkingAction)
            : base(sender)
        {
            this.EWorkingAction = EWorkingAction;
        }

        public EWorkingAction EWorkingAction { get; set; }
    }
}
