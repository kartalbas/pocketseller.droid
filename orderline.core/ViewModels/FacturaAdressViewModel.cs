using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using System.Linq;

namespace pocketseller.core.ViewModels
{
    public class FacturaAdressViewModel : BaseViewModel
    {
        public FacturaAdressViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<OrdersViewServiceMessage>(OnOrdersViewChanged);
        }

        #region Private methods

        private void OnOrdersViewChanged(OrdersViewServiceMessage objMessage)
        {
            if (objMessage.EOrderView == EOrderView.Import_FacturaImported)
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
            LabelTitle = Language.Print;
            LabelMenuTitle = Language.ImportOptionMenu;

            LabelReady = Language.Ready;
            LabelPrintFactura = Language.PrintFactura;
            LabelPrintCash = Language.PrintCash;
            LabelShow = Language.Show;

            LabelDocumentDateTime = Language.Date;
            LabelDocumentNumber = Language.Number;
            LabelDocumentParcels = Language.Parcels;
            LabelDocumentAddressNumber = Language.Adressnumber;
            LabelDocumentZip = Language.Zip;
            LabelDocumentCity = Language.City;
            LabelDocumentName1 = Language.Name;

            Orders = null;

            RemoteDocumentChanged(EOrderState.FACTURAIMPORTED, this);
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelReady;
        public string LabelReady
        {
            get => _labelReady;
            set { _labelReady = value; RaisePropertyChanged(() => LabelReady); }
        }

        private string _labelDelete;
        public string LabelDelete
        {
            get => _labelDelete;
            set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); }
        }

        private string _labelPrintFactura;
        public string LabelPrintFactura
        {
            get => _labelPrintFactura;
            set { _labelPrintFactura = value; RaisePropertyChanged(() => LabelPrintFactura); }
        }

        private string _labelPrintCash;
        public string LabelPrintCash
        {
            get => _labelPrintCash;
            set { _labelPrintCash = value; RaisePropertyChanged(() => LabelPrintCash); }
        }

        private string _labelShow;
        public string LabelShow { get => _labelShow;
            set { _labelShow = value; RaisePropertyChanged(() => LabelShow); } }

        private string _labelPutBack;
        public string LabelPutBack { get => _labelPutBack;
            set { _labelPutBack = value; RaisePropertyChanged(() => LabelPutBack); } }

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

        private MvxCommand<Order> _deleteCommand;
        public ICommand DeleteCommand { get { _deleteCommand = _deleteCommand ?? new MvxCommand<Order>(DoDeleteCommand); return _deleteCommand; } }
        private void DoDeleteCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_Delete);
        }

        private MvxCommand<Order> _readyCommand;
        public ICommand ReadyCommand { get { _readyCommand = _readyCommand ?? new MvxCommand<Order>(DoReadyCommand); return _readyCommand; } }
        private void DoReadyCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_FacturaImported);
        }

        private MvxCommand<Order> _showDocumentCommand;
        public ICommand ShowCommand { get { return _showDocumentCommand = _showDocumentCommand ?? new MvxCommand<Order>(DoShowCommand); } }
        private void DoShowCommand(Order objDocument)
        {
            objDocument.Orderdetails = objDocument?.Orderdetails?.OrderBy(o => o.Pos).ToList();
            DocumentService.Order = objDocument;
            NavigationService.Navigate<StockDocumentViewModel>();
        }

        #endregion
    }
}
