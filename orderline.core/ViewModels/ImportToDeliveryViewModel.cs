using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class ImportToDeliveryViewModel : BaseViewModel
    {
        public ImportToDeliveryViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<OrdersViewServiceMessage>(OnOrdersViewChanged);
        }

        #region Private methods

        private void OnOrdersViewChanged(OrdersViewServiceMessage objMessage)
        {
            if (objMessage.EOrderView == EOrderView.Import_Delivery)
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
            LabelTitle = Language.ToDelivery;
            LabelMenuTitle = Language.ImportOptionMenu;

            LabelImportToDelivery = Language.ToDelivery;
            LabelShow = Language.Show;
            LabelPutBack = Language.PutBack;

            LabelDocumentDateTime = Language.Date;
            LabelDocumentNumber = Language.Number;
            LabelDocumentParcels = Language.Parcels;
            LabelDocumentAddressNumber = Language.Adressnumber;
            LabelDocumentZip = Language.Zip;
            LabelDocumentCity = Language.City;
            LabelDocumentName1 = Language.Name;
            LabelDelete = Language.Delete;

            Orders = null;

            RemoteDocumentChanged(EOrderState.DELIVERY, this);
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelImportToDelivery;
        public string LabelImportToDelivery { get => _labelImportToDelivery;
            set { _labelImportToDelivery = value; RaisePropertyChanged(() => LabelImportToDelivery); } }

        private string _labelDelete;
        public string LabelDelete { get => _labelDelete;
            set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

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

        private MvxCommand<Order> _importCommand;
        public ICommand ImportCommand { get { _importCommand = _importCommand ?? new MvxCommand<Order>(DoImportCommand); return _importCommand; } }
        private void DoImportCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_Delivery);
        }

        private MvxCommand<Order> _PutBackCommand;
        public ICommand PutBackCommand { get { _PutBackCommand = _PutBackCommand ?? new MvxCommand<Order>(DoPutBackCommand); return _PutBackCommand; } }
        private void DoPutBackCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_Delivery);
        }

        private MvxCommand<Order> _deleteCommand;
        public ICommand DeleteCommand { get { _deleteCommand = _deleteCommand ?? new MvxCommand<Order>(DoDeleteCommand); return _deleteCommand; } }
        private void DoDeleteCommand(Order objDocument)
        {
            HandleResult(objDocument, EOrderView.Import_Delivery);
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
