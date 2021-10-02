using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentsViewModel : BaseViewModel
    {
        public DocumentsViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.Order;
            LabelNewOrder = Language.NewOrders;
            LabelSentOrder = Language.SentOrders;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        public void InitDoucment()
        {
            DocumentService.Init();
        }

        private string _labelNewOrder;
        public string LabelNewOrder { get => _labelNewOrder;
            set { _labelNewOrder = value; RaisePropertyChanged(() => LabelNewOrder); } }

        private string _labelSentOrder;
        public string LabelSentOrder { get => _labelSentOrder;
            set { _labelSentOrder = value; RaisePropertyChanged(() => LabelSentOrder); } }

        #endregion
    }
}
