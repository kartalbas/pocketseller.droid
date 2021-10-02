using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
	public class DocumentsNewViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public DocumentsNewViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<DocumentsViewServiceMessage>(OnDocumentsViewChanged);
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
        }

	    #endregion

        #region Private methods

        private void OnDocumentsViewChanged(DocumentsViewServiceMessage objMessage)
        {
            if (objMessage.EDocumentsViewAction == EDocumentsViewAction.Added || objMessage.EDocumentsViewAction == EDocumentsViewAction.Deleted || objMessage.EDocumentsViewAction == EDocumentsViewAction.Updated || objMessage.EDocumentsViewAction == EDocumentsViewAction.SourceChanged)
                Init();
        }

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.NewOrders;
            LabelMenuTitle = Language.DocumentsOptionMenu;

            LabelSendAsOrder = Language.SendAsOrder;
            LabelSendAsCreditNote = Language.SendAsCreditNote;
            LabelSendAsDelivery = Language.SendAsDelivery;
            LabelSendAsFactura = Language.SendAsFactura;
            LabelMail = Language.Mail;
            LabelPrint = Language.Print;
            LabelEdit = Language.Edit;
            LabelDelete = Language.Delete;
            LabelNew = Language.New;

            LabelDocumentDateTime = Language.Date;
            LabelDocumentNumber = Language.Number;
            LabelDocumentParcels = Language.Parcels;
            LabelDocumentAddressNumber = Language.Adressnumber;
            LabelDocumentZip = Language.Zip;
            LabelDocumentCity = Language.City;
            LabelDocumentName1 = Language.Name;

            ListDocuments = Document.FindNewOrChanged();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

		private string _labelEdit;
		public string LabelEdit { get => _labelEdit;
			set { _labelEdit = value; RaisePropertyChanged(() => LabelEdit); } }

		private string _labelSendAsOrder;
		public string LabelSendAsOrder { get => _labelSendAsOrder;
			set { _labelSendAsOrder = value; RaisePropertyChanged(() => LabelSendAsOrder); } }

        private string _labelSendAsDelivery;
        public string LabelSendAsDelivery
        {
            get => _labelSendAsDelivery;
            set { _labelSendAsDelivery = value; RaisePropertyChanged(() => LabelSendAsDelivery); }
        }

        private string _labelSendAsCreditNote;
        public string LabelSendAsCreditNote
        {
            get => _labelSendAsCreditNote;
            set { _labelSendAsCreditNote = value; RaisePropertyChanged(() => LabelSendAsCreditNote); }
        }

        private string _labelSendAsFactura;
        public string LabelSendAsFactura
        {
            get => _labelSendAsFactura;
            set { _labelSendAsFactura = value; RaisePropertyChanged(() => LabelSendAsFactura); }
        }

        private string _labelDelete;
        public string LabelDelete { get => _labelDelete; set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

        private string _labelMail;
        public string LabelMail { get => _labelMail; set { _labelMail = value; RaisePropertyChanged(() => LabelMail); } }

        private string _labelPrint;
        public string LabelPrint { get => _labelPrint; set { _labelPrint = value; RaisePropertyChanged(() => LabelPrint); } }

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

	    private string _labelNew;
	    public string LabelNew { get => _labelNew;
		    set { _labelNew = value; RaisePropertyChanged(() => LabelNew); } }

        public ObservableCollection<Document> ListDocuments { get => Documents;
	        set { Documents = value; RaisePropertyChanged(() => ListDocuments); } }
        #endregion

		#region Public commands

	    private MvxCommand _createNewDocumentCommand;
        public ICommand CreateNewDocumentDocumentCommand { get { _createNewDocumentCommand = _createNewDocumentCommand ?? new MvxCommand(DoCreateNewDocumentCommand); return _createNewDocumentCommand; } }
        private void DoCreateNewDocumentCommand()
        {
            DocumentService.Init();
            NavigateToCommand.Execute(typeof(DocumentViewModel));
	    }

		private MvxCommand<Document> _editDocumentCommand;
		public ICommand EditDocumentCommand { get { return _editDocumentCommand = _editDocumentCommand ?? new MvxCommand<Document>(DoEditDocumentCommand); } }
		private void DoEditDocumentCommand(Document objDoc)
		{
            if (objDoc.State == (int)EOrderState.ORDER)
                objDoc.EditMode = false;
            else
                objDoc.EditMode = true;

            DocumentService.Document = objDoc;
            NavigateToCommand.Execute(typeof(DocumentViewModel));
		}

        private MvxCommand<Document> _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand { get { return _deleteDocumentCommand = _deleteDocumentCommand ?? new MvxCommand<Document>(DoDeleteDocumentCommand); } }
        private void DoDeleteDocumentCommand(Document objDoc)
        {
            DocumentService.DeleteDocument(objDoc);
        }

		#endregion
	}
}
