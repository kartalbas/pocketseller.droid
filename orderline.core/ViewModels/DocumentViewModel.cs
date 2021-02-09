using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using Acr.UserDialogs;
using MvvmCross;

namespace pocketseller.core.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public DocumentViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
        }

        #endregion

        #region Private methods

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.CreateOrder;
            LabelSearch = Language.Search;

            LabelAccount = Language.Account;
            LabelAddress = Language.Address;
            LabelInfo = Language.Info;
            LabelOrder = Language.Order;
            LabelOutstanding = Language.OP;

            LabelTabAccount = Language.TabAccount;
            LabelTabAddress = Language.TabAddress;
            LabelTabInfo = Language.TabInfo;
            LabelTabOrder = Language.TabPositions;
            LabelTabOutstanding = Language.TabOustanding;
        }

        public override void ViewCreated()
        {
            if (!OpenPayment.IsWithinPaymentDays(Document.Adress))
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(
                    Language.OrderNotSavable,
                    Language.Attention,
                    Language.Ok);
            }
            base.ViewCreated();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private DocumentOrderViewModel _documentOrderViewModel;
        public DocumentOrderViewModel DocumentOrderViewModel
        {
            get => _documentOrderViewModel ?? (_documentOrderViewModel = CMvvmCrossTools.LoadViewModel<DocumentOrderViewModel>());
            set => _documentOrderViewModel = value;
        }

        private DocumentAddressViewModel _documentAddressViewModel;
        public DocumentAddressViewModel DocumentAddressViewModel
        {
            get => _documentAddressViewModel ?? (_documentAddressViewModel = CMvvmCrossTools.LoadViewModel<DocumentAddressViewModel>());
            set => _documentAddressViewModel = value;
        }

        private DocumentAccountViewModel _documentAccountViewModel;
        public DocumentAccountViewModel DocumentAccountViewModel
        {
            get => _documentAccountViewModel ?? (_documentAccountViewModel = CMvvmCrossTools.LoadViewModel<DocumentAccountViewModel>());
            set => _documentAccountViewModel = value;
        }

        private DocumentOutstandingViewModel _documentOutstandingViewModel;
        public DocumentOutstandingViewModel DocumentOutstandingViewModel
        {
            get => _documentOutstandingViewModel ?? (_documentOutstandingViewModel = CMvvmCrossTools.LoadViewModel<DocumentOutstandingViewModel>());
            set => _documentOutstandingViewModel = value;
        }

        private DocumentInfoViewModel _documentInfoViewModel;
        public DocumentInfoViewModel DocumentInfoViewModel
        {
            get => _documentInfoViewModel ?? (_documentInfoViewModel = CMvvmCrossTools.LoadViewModel<DocumentInfoViewModel>());
            set => _documentInfoViewModel = value;
        }

        private DocumentdetailViewModel _documentdetailViewModel;
        public DocumentdetailViewModel DocumentdetailViewModel
        {
            get => _documentdetailViewModel ?? (_documentdetailViewModel = CMvvmCrossTools.LoadViewModel<DocumentdetailViewModel>());
            set => _documentdetailViewModel = value;
        }

        private QuotationSelectorViewModel _quotationSelectorViewModel;
        public QuotationSelectorViewModel QuotationSelectorViewModel 
        {
            get => _quotationSelectorViewModel ?? (_quotationSelectorViewModel = CMvvmCrossTools.LoadViewModel<QuotationSelectorViewModel>());
            set => _quotationSelectorViewModel = value;
        }

        private string _labelSearch;
        public string LabelSearch { get => _labelSearch;
            set { _labelSearch = value; RaisePropertyChanged(() => LabelSearch); } }

        private string _labelOrder;
        public string LabelOrder { get => _labelOrder;
            set { _labelOrder = value; RaisePropertyChanged(() => LabelOrder); } }

        private string _labelInfo;
        public string LabelInfo { get => _labelInfo;
            set { _labelInfo = value; RaisePropertyChanged(() => LabelInfo); } }

        private string _labelAddress;
        public string LabelAddress { get => _labelAddress;
            set { _labelAddress = value; RaisePropertyChanged(() => LabelAddress); } }

        private string _labelAccount;
        public string LabelAccount { get => _labelAccount;
            set { _labelAccount = value; RaisePropertyChanged(() => LabelAccount); } }

        private string _labelOutstanding;
        public string LabelOutstanding { get => _labelOutstanding;
            set { _labelOutstanding = value; RaisePropertyChanged(() => LabelOutstanding); } }

        private string _labelTabOrder;
        public string LabelTabOrder { get => _labelTabOrder;
            set { _labelTabOrder = value; RaisePropertyChanged(() => LabelTabOrder); } }

        private string _labelTabInfo;
        public string LabelTabInfo { get => _labelTabInfo;
            set { _labelTabInfo = value; RaisePropertyChanged(() => LabelTabInfo); } }

        private string _labelTabAddress;
        public string LabelTabAddress { get => _labelTabAddress;
            set { _labelTabAddress = value; RaisePropertyChanged(() => LabelTabAddress); } }

        private string _labelTabAccount;
        public string LabelTabAccount { get => _labelTabAccount;
            set { _labelTabAccount = value; RaisePropertyChanged(() => LabelTabAccount); } }

        private string _labelTabOutstanding;
        public string LabelTabOutstanding { get => _labelTabOutstanding;
            set { _labelTabOutstanding = value; RaisePropertyChanged(() => LabelTabOutstanding); } }

        public EOrderState DocumentState => (EOrderState)DocumentService.Document.State;

        public Document Document => DocumentService.Document;

        public Adress Address => DocumentService.Document.Adress;

        public bool IsInPaymentState => OpenPayment.IsWithinPaymentDays(Address);

        public bool IsSavable
        {
            get
            {
                if (Address == null)
                    return false;

                if (string.IsNullOrEmpty(Address.Adressnumber))
                    return false;

                if (Document == null)
                    return false;

                if (Document.Documentdetails == null)
                    return false;

                if (Document.Documentdetails.Count == 0)
                    return false;

                return IsInPaymentState;
            }
        }

        #endregion

        #region ICommand implementations
        #endregion
    }
}
