using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum ELanguage
    {
        tr_TR,
        en_GB,
        de_DE
    }

    public class LanguageServiceMessage : MvxMessage
    {
        public LanguageServiceMessage(object sender, bool initializeLabels)
            : base(sender)
        {
            InitializeLabels = initializeLabels;
        }

        public bool InitializeLabels { get; set; }
    }
}
