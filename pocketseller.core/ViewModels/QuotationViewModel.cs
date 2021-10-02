using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using Quotation = pocketseller.core.Models.Quotation;
using Quotationdetail = pocketseller.core.Models.Quotationdetail;

namespace pocketseller.core.ViewModels
{
	public class QuotationViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public QuotationViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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

        private void SearchNow()
        {
            if (SearchKey.Length > 0 && SearchKey.Length >= SettingService.Get<int>(ESettingType.SearchMaxChar) && SearchKey.Length < 20)
                ListArticles = Article.Find(SearchKey);
            else
                ListArticles = new List<Article>();
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.Quotations;
            LabelMenuTitle = Language.QutotationValidOptionMenu;

            LabelStartDate = Language.DateStart;
            LabelStopDate = Language.DateStop;

            LabelPos = Language.Pos;
            LabelArticlenumber = Language.Articlenumber;
            LabelCount = Language.Count;
            LabelContent = Language.Content;
            LabelAmount = Language.Amount;
            LabelUnitPrice = Language.Price;
            LabelArticle = Language.Article;
            LabelPositionTotal = Language.PositionTotal;

            LabelSearch = Language.Search;
            LabelHint = Language.SearchArticle;
            LabelDelete = Language.Delete;
            LabelEdit = Language.Edit;

            ListArticles = new List<Article>();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        public EOrderState DocumentState
	    {
            get => (EOrderState)DocumentService.Quotation.State;
            set => DocumentService.Quotation.State = (int)value;
        }

        private DocumentdetailViewModel _documentdetailViewModel;
        public DocumentdetailViewModel DocumentdetailViewModel
        {
            get
            {
                var result = _documentdetailViewModel ?? (_documentdetailViewModel = CMvvmCrossTools.LoadViewModel<DocumentdetailViewModel>());
                result.Mode = CDocumentService.EMode.Quotation;
                return result;
            }

            set => _documentdetailViewModel = value;
        }

        private string _labelHint;
        public string LabelHint
        {
            get => _labelHint;
            set
            {
                if (!string.IsNullOrEmpty(_labelHint))
                    return;

                _labelHint = value; RaisePropertyChanged(() => LabelHint);
            }
        }

        private string _labelEdit;
        public string LabelEdit { get => _labelEdit;
            set { _labelEdit = value; RaisePropertyChanged(() => LabelEdit); } }

        private string _labelDelete;
        public string LabelDelete { get => _labelDelete;
            set { _labelDelete = value; RaisePropertyChanged(() => LabelDelete); } }

        private string _labelSearch;
        public string LabelSearch { get => _labelSearch;
            set { _labelSearch = value; RaisePropertyChanged(() => LabelSearch); } }

        private string _labelStartDate;
        public string LabelStartDate { get => _labelStartDate;
            set { _labelStartDate = value; RaisePropertyChanged(() => LabelStartDate); } }

        private string _labelStopDate;
        public string LabelStopDate { get => _labelStopDate;
            set { _labelStopDate = value; RaisePropertyChanged(() => LabelStopDate); } }

        private string _labelPos;
        public string LabelPos { get => _labelPos;
            set { _labelPos = value; RaisePropertyChanged(() => LabelPos); } }

        private string _labelCount;
        public string LabelCount { get => _labelCount;
            set { _labelCount = value; RaisePropertyChanged(() => LabelCount); } }

        private string _labelContent;
        public string LabelContent { get => _labelContent;
            set { _labelContent = value; RaisePropertyChanged(() => LabelContent); } }

        private string _labelAmount;
        public string LabelAmount { get => _labelAmount;
            set { _labelAmount = value; RaisePropertyChanged(() => LabelAmount); } }

        private string _labelUnitPrice;
        public string LabelUnitPrice { get => _labelUnitPrice;
            set { _labelUnitPrice = value; RaisePropertyChanged(() => LabelUnitPrice); } }

        private string _labelArticlenumber;
        public string LabelArticlenumber { get => _labelArticlenumber;
            set { _labelArticlenumber = value; RaisePropertyChanged(() => LabelArticlenumber); } }

        private string _labelArticle;
        public string LabelArticle { get => _labelArticle;
            set { _labelArticle = value; RaisePropertyChanged(() => LabelArticle); } }

        private string _labelPositionTotal;
        public string LabelPositionTotal { get => _labelPositionTotal;
            set { _labelPositionTotal = value; RaisePropertyChanged(() => LabelPositionTotal); } }

        private string _searchKey;
        public string SearchKey
        {
            get => _searchKey;
            set
            {
                if (typeof(Article).ToString() != value)
                {
                    _searchKey = value;
                    SearchNow();
                }
                else
                {
                    _searchKey = string.Empty;
                }

                RaisePropertyChanged(() => SearchKey);
            }
        }

        private Article _article;
        public Article Article
        {
            get => _article;
            set
            {
                _article = value;
                DocumentService.ChangeQuotationdetailState(EOrderdetailState.NEW); //important must be set BEFORE article!
                DocumentService.Quotationdetail.Article = Article;
                RaisePropertyChanged(() => Article);
                Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentDetail, ActionMode.Quotation));
            }
        }

        private List<Article> _listArticles;
        public List<Article> ListArticles { get => _listArticles;
            set { _listArticles = value; RaisePropertyChanged(() => ListArticles); } }

        public Quotation Quotation => DocumentService.Quotation;

        public ObservableCollection<Quotationdetail> Quotationdetails
        {
            get => DocumentService.Quotation.Quotationdetails;
            set
            {
                DocumentService.Quotation.Quotationdetails = value;
                RaisePropertyChanged(() => Quotationdetails);
            }
        }

        public bool IsSavable
        {
            get
            {
                if (Quotation == null)
                    return false;

                if (Quotation.Quotationdetails == null)
                    return false;

                if (Quotation.Quotationdetails.Count == 0)
                    return false;

                return true;
            }
        }

        public ESettingType KeyboardSetting => (ESettingType)SettingService.Get<int>(ESettingType.KeyboardTypeOnSearch);

        #endregion

        #region ICommand implementations

        private MvxCommand<Quotationdetail> _positionEditCommand;
        public ICommand PositionEditCommand
        {
            get
            {
                _positionEditCommand = _positionEditCommand ?? new MvxCommand<Quotationdetail>(DoPositionEditCommand);
                return _positionEditCommand;
            }
        }
        private void DoPositionEditCommand(Quotationdetail objDocDetail)
        {
            objDocDetail.State = (int)EOrderdetailState.EDIT;
            DocumentService.Quotationdetail = objDocDetail;
            Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentDetail));
        }

        private MvxCommand<Quotationdetail> _positionDeleteCommand;
        public ICommand PositionDeleteCommand
        {
            get
            {
                _positionDeleteCommand = _positionDeleteCommand ?? new MvxCommand<Quotationdetail>(DoPositionDeleteCommand);
                return _positionDeleteCommand;
            }
        }
        private void DoPositionDeleteCommand(Quotationdetail objDocDetail)
        {
            DocumentService.Quotationdetail = objDocDetail;
            DocumentService.RemoveQuotationdetail();
        }

        private MvxCommand _saveDocumentCommand;
        public ICommand SaveDocumentCommand { get { _saveDocumentCommand = _saveDocumentCommand ?? new MvxCommand(DoSaveDocumentCommand); return _saveDocumentCommand; } }
        private void DoSaveDocumentCommand()
        {
            DocumentService.SaveQuotation();
        }

        private MvxCommand _discarDocumentCommand;
        public ICommand DiscarDocumentCommand { get { _discarDocumentCommand = _discarDocumentCommand ?? new MvxCommand(DoDiscarDocumentCommand); return _discarDocumentCommand; } }
        private void DoDiscarDocumentCommand()
        {
            DocumentService.DiscarDocument();
        }

	    private MvxCommand _setDefaultDatesCommand;
        public ICommand SetDefaultDatesCommand { get { _setDefaultDatesCommand = _setDefaultDatesCommand ?? new MvxCommand(DoSetDefaultDatesCommand); return _setDefaultDatesCommand; } }
        private void DoSetDefaultDatesCommand()
	    {
            if (DocumentService.Quotation.StartDateTime == default(DateTime))
            {
                var objDate = DateTime.Now;
                DocumentService.Quotation.StartDateTime = new DateTime(objDate.Year, objDate.Month, objDate.Day, 0, 0, 0);
                objDate = objDate.AddDays(1);
                DocumentService.Quotation.StopDateTime = new DateTime(objDate.Year, objDate.Month, objDate.Day, 0, 0, 0);
            }
	    }

        #endregion
    }
}
