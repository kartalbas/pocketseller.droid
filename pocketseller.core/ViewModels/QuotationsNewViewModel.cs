using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using Quotation = pocketseller.core.Models.Quotation;

namespace pocketseller.core.ViewModels
{
    public class QuotationsNewViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public QuotationsNewViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.NewQuotations;
            LabelMenuTitle = Language.QutotationsNewOptionMenu;

            LabelSend = Language.Send;
            LabelEdit = Language.Edit;
            LabelDelete = Language.Delete;

            LabelStartDate = Language.DateStart;
            LabelStopDate = Language.DateStop;
            LabelQuotationNr = Language.QuotationsNr;

            ListDocuments = Quotation.FindNewOrChanged();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelSend;
        public string LabelSend { get => _labelSend;
            set { _labelSend = value; RaisePropertyChanged(() => LabelSend); } }
        
        private string _labelEdit;
		public string LabelEdit { get => _labelEdit;
            set { _labelEdit = value; RaisePropertyChanged(() => LabelEdit); } }

        private string _labelDelete;
        public string LabelDelete { get => _labelDelete;
            set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

        private string _labelStartDate;
        public string LabelStartDate { get => _labelStartDate;
            set { _labelStartDate = value; RaisePropertyChanged(() => LabelStartDate); } }

        private string _labelStopDate;
        public string LabelStopDate { get => _labelStopDate;
            set { _labelStopDate = value; RaisePropertyChanged(() => LabelStopDate); } }

        private string _labelQuotationNr;
        public string LabelQuotationNr { get => _labelQuotationNr;
            set { _labelQuotationNr = value; RaisePropertyChanged(() => LabelQuotationNr); } }

        public ObservableCollection<Quotation> ListDocuments { get => Quotations;
            set { Quotations = value; RaisePropertyChanged(() => ListDocuments); } }

        #endregion

        #region Public commands

        private MvxCommand _createNewDocumentCommand;
        public ICommand CreateNewDocumentDocumentCommand { get { _createNewDocumentCommand = _createNewDocumentCommand ?? new MvxCommand(DoCreateNewDocumentCommand); return _createNewDocumentCommand; } }
        private void DoCreateNewDocumentCommand()
        {
            DocumentService.Init();
            NavigateToCommand.Execute(typeof(QuotationViewModel));
        }

        private MvxCommand<Quotation> _editDocumentCommand;
        public ICommand EditDocumentCommand { get { return _editDocumentCommand = _editDocumentCommand ?? new MvxCommand<Quotation>(DoEditDocumentCommand); } }
        private void DoEditDocumentCommand(Quotation objDoc)
        {
            if (objDoc.State == (int)EOrderState.ORDER)
                objDoc.EditMode = false;
            else
                objDoc.EditMode = true;

            DocumentService.Quotation = objDoc;
            NavigateToCommand.Execute(typeof(QuotationViewModel));
        }

        //private MvxCommand<Quotation> _sendDocumentCommand;
        //public ICommand SendDocumentCommand { get { return _sendDocumentCommand = _sendDocumentCommand ?? new MvxCommand<Quotation>(DoSendDocumentCommand); } }
        //private void DoSendDocumentCommand(Quotation objDocument)
        //{
        //    if (objDocument != null)
        //    {
        //        int iDocumentNumber = 0;
        //        int.TryParse(objDocument.Response.Replace("\"", ""), out iDocumentNumber);

        //        if (iDocumentNumber > 0)
        //        {
        //            Quotation.ChangePhase(objDocument, EPhaseState.SERVER);
        //            OrderSettings.Instance.CurrentQuoNr = iDocumentNumber;
        //            Quotation.UpdateQuotationnumber(objDocument, iDocumentNumber);
        //            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        //        }
        //        else
        //        {
        //            Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.QuotationCouldNotBeSent + " >>> ERROR MESSAGE: " + objDocument.Response, Language.Attention);
        //        }
        //    }
        //    else
        //    {
        //        Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.QuotationCouldNotBeSent + " >>> ERROR MESSAGE: Document = NULL", Language.Attention);
        //    }
        //}

        private MvxCommand<Quotation> _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand { get { return _deleteDocumentCommand = _deleteDocumentCommand ?? new MvxCommand<Quotation>(DoDeleteDocumentCommand); } }
        private void DoDeleteDocumentCommand(Quotation objDoc)
        {
            DocumentService.DeleteQuotation(objDoc);
        }

        #endregion
    }
}
