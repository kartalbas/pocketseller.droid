using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
	public class QuotationSelectorViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public QuotationSelectorViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
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
            LabelTitle = Language.Quotations;
            LabelMenuTitle = string.Empty;
            LabelCancel = Language.Cancel;
            LabelTakeOver = Language.TakeOver;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelCancel;
        public string LabelCancel { get => _labelCancel;
            set { _labelCancel = value; RaisePropertyChanged(() => LabelCancel); } }
        
	    private string _labelTakeOver;
	    public string LabelTakeOver { get => _labelTakeOver;
            set { _labelTakeOver = value; RaisePropertyChanged(() => LabelTakeOver); } }

        public Quotation Quotation
	    {
	        get => DocumentService.Quotation;
            set
	        {
	            DocumentService.Quotation = value;
                RaisePropertyChanged(() => Quotation);
	        }
	    }

        public ObservableCollection<Quotationdetail> Quotationdetails
        {
            get => DocumentService.Quotation.Quotationdetails;
            set
            {
                DocumentService.Quotation.Quotationdetails = value;
                RaisePropertyChanged(() => Quotationdetails);
            }
        }

        #endregion

        #region ICommand implementations

	    private MvxCommand _takeOverCommand;
        public ICommand TakeOverCommand { get { _takeOverCommand = _takeOverCommand ?? new MvxCommand(DoTakeOverCommand); return _takeOverCommand; } }
        private void DoTakeOverCommand()
        {
            DocumentService.CopyQuototationdetailsToOrderdetails();
        }


	    private MvxCommand _showValidQuotationsCommand;
        public ICommand ShowValidQuotationsCommand { get { _showValidQuotationsCommand = _showValidQuotationsCommand ?? new MvxCommand(DoShowValidQuotationsCommand); return _showValidQuotationsCommand; } }
        private void DoShowValidQuotationsCommand()
        {
            var quotationdetails = new ObservableCollection<Quotationdetail>();
            var quotations = Quotation.FindValid();
            foreach (var quotation in quotations)
            {
                foreach (var quotationdetail in quotation.Quotationdetails)
                {
                    quotationdetails.Add(quotationdetail);
                }
            }

            Quotationdetails = quotationdetails;
        }

        #endregion
    }
}
