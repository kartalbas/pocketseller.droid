using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelConverter;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using Quotation = pocketseller.core.Models.Quotation;

namespace pocketseller.core.ViewModels
{
	public class QuotationsSentViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public QuotationsSentViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            if (objMessage.EDocumentsViewAction == EDocumentsViewAction.Added || objMessage.EDocumentsViewAction == EDocumentsViewAction.Deleted || objMessage.EDocumentsViewAction == EDocumentsViewAction.Updated)
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
            LabelTitle = Language.SentQuotations;
            LabelMenuTitle = Language.QutotationsSentOptionMenu;

            LabelDelete = Language.Delete;

            LabelStartDate = Language.DateStart;
            LabelStopDate = Language.DateStop;
            LabelQuotationNr = Language.QuotationsNr;

            ListDocuments = Quotation.FindSent();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

		private string _labelEdit;
		public string LabelEdit { get => _labelEdit;
			set { _labelEdit = value; RaisePropertyChanged(() => LabelEdit); } }

		private string _labelSend;
		public string LabelSend { get => _labelSend;
			set { _labelSend = value; RaisePropertyChanged(() => LabelSend); } }

        private string _labelDelete;
        public string LabelDelete { get => _labelDelete;
	        set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

        private string _labelNew;
        public string LabelNew { get => _labelNew;
	        set { _labelNew = value; RaisePropertyChanged(() => LabelNew); } }

        private string _labelQuotationNr;
        public string LabelQuotationNr { get => _labelQuotationNr;
	        set { _labelQuotationNr = value; RaisePropertyChanged(() => LabelQuotationNr); } }

        private string _labelStartDate;
        public string LabelStartDate { get => _labelStartDate;
	        set { _labelStartDate = value; RaisePropertyChanged(() => LabelStartDate); } }

        private string _labelStopDate;
        public string LabelStopDate { get => _labelStopDate;
	        set { _labelStopDate = value; RaisePropertyChanged(() => LabelStopDate); } }

        public ObservableCollection<Quotation> ListDocuments { get => Quotations;
	        set { Quotations = value; RaisePropertyChanged(() => ListDocuments); } }

		#endregion

        #region Public commands

        private MvxCommand<Quotation> _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand { get { return _deleteDocumentCommand = _deleteDocumentCommand ?? new MvxCommand<Quotation>(DoDeleteDocumentCommand); } }
        private void DoDeleteDocumentCommand(Quotation objDoc)
        {
            DocumentService.DeleteQuotation(objDoc);
            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }

        private MvxCommand<ObservableCollection<ModelsAPI.Quotation>> _replaceCommand;
        public ICommand ReplaceCommand { get { _replaceCommand = _replaceCommand ?? new MvxCommand<ObservableCollection<ModelsAPI.Quotation>>(DoReplaceCommand); return _replaceCommand; } }
	    private void DoReplaceCommand(ObservableCollection<ModelsAPI.Quotation> cobjQuotations)
	    {
            Quotation.DeleteQuotations(EPhaseState.SERVER);

            foreach (var objQuotation in cobjQuotations)
	        {
                var cobjNewQuotations = ProxyQuoToQuotation.CreateQuotation(objQuotation);
                cobjNewQuotations.SaveOrUpdate();
	        }

            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }

        #endregion
    }
}
