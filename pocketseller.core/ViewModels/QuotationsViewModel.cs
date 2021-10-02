using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class QuotationsViewModel : BaseViewModel
    {
        public QuotationsViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
        }

        #region Private methods

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methiods

        public override void Init()
        {
            LabelTitle = Language.Quotations;
            LabelNewQuotations = Language.NewQuotations;
            LabelSentQuotations = Language.SentQuotations;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelNewQuotations;
        public string LabelNewQuotations { get => _labelNewQuotations;
            set { _labelNewQuotations = value; RaisePropertyChanged(() => LabelNewQuotations); } }

        private string _labelSentQuotations;
        public string LabelSentQuotations { get => _labelSentQuotations;
            set { _labelSentQuotations = value; RaisePropertyChanged(() => LabelSentQuotations); } }

        #endregion
    }
}
