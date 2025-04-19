using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class ImportToFacturaViewModel : BaseViewModel
    {
        public ImportToFacturaViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;

             SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<OrdersViewServiceMessage>(OnOrdersViewChanged);
        }

        #region Private methods

        private void OnOrdersViewChanged(OrdersViewServiceMessage objMessage)
        {
            if (objMessage.EOrderView == EOrderView.Import_Factura)
                Init();
            else if (objMessage.EOrderView == EOrderView.SourceChanged)
                Orders = null;
        }

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methiods

        public override void Init()
        {
            LabelTitle = Language.ToFactura;
            LabelMenuTitle = Language.DeleteOptionMenu;

            LabelImportToFactura = Language.ToFactura;
            LabelImportAsCreditNote = Language.ToCreditNote;
            LabelPrintDeliveryNoteWithPrice = Language.PrintDeliveryNoteWithPrice;
            LabelPrintDeliveryNoteWithoutPrice = Language.PrintDeliveryNoteWithoutPrice;
            LabelShow = Language.Show;

            LabelDocumentDateTime = Language.Date;
            LabelDocumentNumber = Language.Number;
            LabelDocumentParcels = Language.Parcels;
            LabelDocumentAddressNumber = Language.Adressnumber;
            LabelDocumentZip = Language.Zip;
            LabelDocumentCity = Language.City;
            LabelDocumentName1 = Language.Name;

            Orders = null;

            RemoteDocumentChanged(EOrderState.FACTURA, this);
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelImportAsCreditNote;
        public string LabelImportAsCreditNote
        {
            get => _labelImportAsCreditNote;
            set { _labelImportAsCreditNote = value; RaisePropertyChanged(() => LabelImportAsCreditNote); }
        }

        private string _labelPrintDeliveryNoteWithPrice;
        public string LabelPrintDeliveryNoteWithPrice
        {
            get => _labelPrintDeliveryNoteWithPrice;
            set { _labelPrintDeliveryNoteWithPrice = value; RaisePropertyChanged(() => LabelPrintDeliveryNoteWithPrice); }
        }

        private string _labelPrintDeliveryNoteWithoutPrice;
        public string LabelPrintDeliveryNoteWithoutPrice
        {
            get => _labelPrintDeliveryNoteWithoutPrice;
            set { _labelPrintDeliveryNoteWithoutPrice = value; RaisePropertyChanged(() => LabelPrintDeliveryNoteWithoutPrice); }
        }

        private string _labelImportToFactura;
        public string LabelImportToFactura
        {
            get => _labelImportToFactura;
            set { _labelImportToFactura = value; RaisePropertyChanged(() => LabelImportToFactura); }
        }

        private string _labelShow;
        public string LabelShow { get => _labelShow;
            set { _labelShow = value; RaisePropertyChanged(() => LabelShow); } }

        private string _labelDocumentDateTime;
        public string LabelDocumentDateTime { get => _labelDocumentDateTime;
            set { _labelDocumentDateTime = value; RaisePropertyChanged(() => LabelDocumentDateTime); } }

        private string _labelDocumentNumber;
        public string LabelDocumentNumber { get => _labelDocumentNumber;
            set { _labelDocumentNumber = value; RaisePropertyChanged(() => LabelDocumentNumber); } }

        private string _labelDocumentParcels;
        public string LabelDocumentParcels { get => _labelDocumentParcels;
            set { _labelDocumentParcels = value; RaisePropertyChanged(() => LabelDocumentParcels); } }

        private string _labelDocumentAddressNumber;
        public string LabelDocumentAddressNumber { get => _labelDocumentAddressNumber;
            set { _labelDocumentAddressNumber = value; RaisePropertyChanged(() => LabelDocumentAddressNumber); } }

        private string _labelDocumentZip;
        public string LabelDocumentZip { get => _labelDocumentZip;
            set { _labelDocumentZip = value; RaisePropertyChanged(() => LabelDocumentZip); } }

        private string _labelDocumentCity;
        public string LabelDocumentCity { get => _labelDocumentCity;
            set { _labelDocumentCity = value; RaisePropertyChanged(() => LabelDocumentCity); } }

        private string _labelDocumentName1;
        public string LabelDocumentName1 { get => _labelDocumentName1;
            set { _labelDocumentName1 = value; RaisePropertyChanged(() => LabelDocumentName1); } }

        private MvxCommand<Order> _importCommand;
        public ICommand ImportCommand { get { _importCommand = _importCommand ?? new MvxCommand<Order>(DoImportCommand); return _importCommand; } }
        private void DoImportCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_Factura);
        }

        private MvxCommand<Order> _showDocumentCommand;
        public ICommand ShowCommand { get { return _showDocumentCommand = _showDocumentCommand ?? new MvxCommand<Order>(DoShowCommand); } }
        private void DoShowCommand(Order objDocument)
        {
            DocumentService.Order = objDocument;
            NavigationService.Navigate<StockDocumentViewModel>();
        }

        #endregion
    }
}
