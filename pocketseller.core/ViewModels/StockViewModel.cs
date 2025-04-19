using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class StockViewModel : BaseViewModel
    {
        public StockViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.Stock;
            LabelComission = Language.Comission;
            LabelReady = Language.Ready;
            LabelDelivery = Language.ToDelivery;
            LabelFactura = Language.ToFactura;
            LabelToDelete = Language.ToPrint;
            LabelToCanceled = Language.ToCanceled;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelToDeficit;
        public string LabelToDeficit { get => _labelToDeficit; set { _labelToDeficit = value; RaisePropertyChanged(() => LabelToDeficit); } }

        private string _labelComission;
        public string LabelComission { get => _labelComission; set { _labelComission = value; RaisePropertyChanged(() => LabelComission); } }

        private string _labelReady;
        public string LabelReady { get => _labelReady; set { _labelReady = value; RaisePropertyChanged(() => LabelReady); } }

        private string _labelFactura;
        public string LabelFactura { get => _labelFactura; set { _labelFactura = value; RaisePropertyChanged(() => LabelFactura); } }

        private string _labelToDelete;
        public string LabelToDelete { get => _labelToDelete; set { _labelToDelete = value; RaisePropertyChanged(() => LabelToDelete); } }

        private string _labelDelivery;
        public string LabelDelivery { get => _labelDelivery; set { _labelDelivery = value; RaisePropertyChanged(() => LabelDelivery); } }

        private string _labelToCanceled;
        public string LabelToCanceled { get => _labelToCanceled; set { _labelToCanceled = value; RaisePropertyChanged(() => LabelToCanceled); } }

        #endregion
    }
}
