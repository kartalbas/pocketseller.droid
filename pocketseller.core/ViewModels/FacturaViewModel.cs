using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class FacturaViewModel : BaseViewModel
    {
        public FacturaViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.PrintFactura;
            LabelFacturaDefect = Language.FacturaDefect;
            LabelFacturaAdress = Language.FacturaKonto;
            LabelFacturaDay = Language.FacturaDays;
            LabelFacturaWeek = Language.FacturaWeek;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelFacturaAdress;
        public string LabelFacturaAdress { get => _labelFacturaAdress; set { _labelFacturaAdress = value; RaisePropertyChanged(() => LabelFacturaAdress); } }

        private string _labelFacturaDay;
        public string LabelFacturaDay { get => _labelFacturaDay; set { _labelFacturaDay = value; RaisePropertyChanged(() => LabelFacturaDay); } }

        private string _labelFacturaWeek;
        public string LabelFacturaWeek { get => _labelFacturaWeek; set { _labelFacturaWeek = value; RaisePropertyChanged(() => LabelFacturaWeek); } }

        private string _labelFacturaDefect;
        public string LabelFacturaDefect { get => _labelFacturaDefect; set { _labelFacturaDefect = value; RaisePropertyChanged(() => LabelFacturaDefect); } }
        #endregion
    }
}
