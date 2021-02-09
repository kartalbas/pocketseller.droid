using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using orderline.core.Resources.Languages;
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
            LabelToPrint = Language.Comission;
            LabelToDelete = Language.ToPrint;
            LabelToDelivery = Language.ToDelivery;
            LabelToFactura = Language.ToFactura;
            LabelToCanceled = Language.ToCanceled;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelToDeficit;
        public string LabelToDeficit { get => _labelToDeficit; set { _labelToDeficit = value; RaisePropertyChanged(() => LabelToDeficit); } }

        private string _labelToPrint;
        public string LabelToPrint { get => _labelToPrint; set { _labelToPrint = value; RaisePropertyChanged(() => LabelToPrint); } }

        private string _labelToDelete;
        public string LabelToDelete { get => _labelToDelete; set { _labelToDelete = value; RaisePropertyChanged(() => LabelToDelete); } }

        private string _labelToDelivery;
        public string LabelToDelivery { get => _labelToDelivery; set { _labelToDelivery = value; RaisePropertyChanged(() => LabelToDelivery); } }

        private string _labelToFactura;
        public string LabelToFactura { get => _labelToFactura; set { _labelToFactura = value; RaisePropertyChanged(() => LabelToFactura); } }

        private string _labelToCanceled;
        public string LabelToCanceled { get => _labelToCanceled; set { _labelToCanceled = value; RaisePropertyChanged(() => LabelToCanceled); } }

        #endregion
    }
}
