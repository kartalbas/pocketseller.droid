using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class ImportViewModel : BaseViewModel
    {
        public ImportViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.Import;
            LabelToImport = Language.ToDelivery;
            LabelToDelivery = Language.ToDelivery;
            LabelToDeleted = Language.ToDeleted;
            LabelToFactura = Language.ToFactura;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelToImport;
        public string LabelToImport { get => _labelToImport;
            set { _labelToImport = value; RaisePropertyChanged(() => LabelToImport); } }

        private string _labelToFactura;
        public string LabelToFactura
        {
            get => _labelToFactura;
            set { _labelToFactura = value; RaisePropertyChanged(() => LabelToFactura); }
        }

        private string _labelToDelivery;
        public string LabelToDelivery { get => _labelToDelivery;
            set { _labelToDelivery = value; RaisePropertyChanged(() => LabelToDelivery); } }

        private string _labelToDeleted;
        public string LabelToDeleted { get => _labelToDeleted;
            set { _labelToDeleted = value; RaisePropertyChanged(() => LabelToDeleted); } }

        #endregion
    }
}
