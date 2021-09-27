using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentsSentViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public DocumentsSentViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.SentOrders;
            LabelMenuTitle = Language.DocumentsOptionMenu;

            LabelDelete = Language.Delete;
            LabelActivate = Language.Select;

            LabelDocumentDateTime = Language.Date;
            LabelDocumentNumber = Language.Number;
            LabelDocumentParcels = Language.Parcels;
            LabelDocumentAddressNumber = Language.Adressnumber;
            LabelDocumentZip = Language.Zip;
            LabelDocumentCity = Language.City;
            LabelDocumentName1 = Language.Name;

            ListDocuments = Document.FindSent();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

		private string _labelDelete;
		public string LabelDelete { get => _labelDelete;
			set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

		private string _labelActivate;
		public string LabelActivate { get => _labelActivate;
			set { _labelActivate = value; RaisePropertyChanged(() => LabelActivate); } }

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

        public ObservableCollection<Document> ListDocuments { get => Documents;
	        set { Documents = value; RaisePropertyChanged(() => ListDocuments); } }

		#endregion

		#region Public commands

        private MvxCommand<Document> _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand { get { return _deleteDocumentCommand = _deleteDocumentCommand ?? new MvxCommand<Document>(DoDeleteDocumentCommand); } }
        private void DoDeleteDocumentCommand(Document objDoc)
        {
            DocumentService.DeleteDocument(objDoc);
            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }

		private MvxCommand<Document> _activateDocumentCommand;
		public ICommand ActivateDocumentCommand { get { return _activateDocumentCommand = _activateDocumentCommand ?? new MvxCommand<Document>(DoActivateDocumentCommand); } }
		private void DoActivateDocumentCommand(Document objDoc)
		{
			Document.ChangePhase(objDoc, EPhaseState.ACTIVATED);
            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }

        #endregion
	}
}
