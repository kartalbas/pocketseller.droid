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

namespace pocketseller.core.ViewModels
{
    public class DocumentOrderViewModel : BaseViewModel
    {
        #region Private properties

        #endregion

        #region Constructors

        public DocumentOrderViewModel(IDataService objDataService, IDocumentService objDocumentService,
            ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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

        private void SearchNow()
        {
            if (SearchKey.Length > 0 && SearchKey.Length >= SettingService.Get<int>(ESettingType.SearchMaxChar) &&
                SearchKey.Length < 20)
                ListArticles = Article.Find(SearchKey);
            else
                ListArticles = new List<Article>();
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.TabPositions;
            LabelMenuTitle = Language.DocumentdetailsOptionMenu;

            LabelDelete = Language.Delete;
            LabelEdit = Language.Edit;
            LabelInfo = Language.Info;
            LabelOp = Language.OP;
            LabelParcels = Language.Parcels;
            LabelTotalVat = Language.OrderValue;
            LabelTotalBrutto = Language.OrderValueVat;
            LabelPositionTotal = Language.PositionTotal;
            LabelDate = Language.Date;

            LabelPos = Language.Pos;
            LabelArticlenumber = Language.Articlenumber;
            LabelCount = Language.Count;
            LabelContent = Language.Content;
            LabelAmount = Language.Amount;
            LabelUnitPrice = Language.Price;
            LabelArticle = Language.Article;

            LabelSearch = Language.SearchArticle;
            LabelButtonSearch = Language.Search;

            LabelButtonSearch = Language.SearchArticle;
            LabelName1 = Language.Name1;
            LabelSearch = Language.Search;
            LabelHint = Language.SearchArticle;
            LabelStock = Language.CurrentStockCount;

            Date = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";

            ListArticles = new List<Article>();
        }

        public override void Init(object objParam)
        {
        }

        #endregion

        #region Public properties

        private string _labelHint;

        public string LabelHint
        {
            get => _labelHint;
            set
            {
                if (!string.IsNullOrEmpty(_labelHint))
                    return;

                _labelHint = value;
                RaisePropertyChanged(() => LabelHint);
            }
        }

        private bool _progressStatus;

        public bool ProgressStatus
        {
            get => _progressStatus;
            set
            {
                _progressStatus = value;
                RaisePropertyChanged(() => ProgressStatus);
            }
        }

        private bool _progressVisibility;

        public bool ProgressVisibility
        {
            get => _progressVisibility;
            set
            {
                _progressVisibility = value;
                RaisePropertyChanged(() => ProgressVisibility);
            }
        }

        private string _date;

        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        private string _labelDate;

        public string LabelDate
        {
            get => _labelDate;
            set
            {
                _labelDate = value;
                RaisePropertyChanged(() => LabelDate);
            }
        }

        private string _labelEdit;

        public string LabelEdit
        {
            get => _labelEdit;
            set
            {
                _labelEdit = value;
                RaisePropertyChanged(() => LabelEdit);
            }
        }

        private string _labelDelete;

        public string LabelDelete
        {
            get => _labelDelete;
            set
            {
                _labelDelete = value;
                RaisePropertyChanged(() => LabelDelete);
            }
        }

        private string _labelSearch;

        public string LabelSearch
        {
            get => _labelSearch;
            set
            {
                _labelSearch = value;
                RaisePropertyChanged(() => LabelSearch);
            }
        }

        private string _labelInfo;

        public string LabelInfo
        {
            get => _labelInfo;
            set
            {
                _labelInfo = value;
                RaisePropertyChanged(() => LabelInfo);
            }
        }

        private string _labelOp;

        public string LabelOp
        {
            get => _labelOp;
            set
            {
                _labelOp = value;
                RaisePropertyChanged(() => LabelOp);
            }
        }

        private string _labelArticlenumber;

        public string LabelArticlenumber
        {
            get => _labelArticlenumber;
            set
            {
                _labelArticlenumber = value;
                RaisePropertyChanged(() => LabelArticlenumber);
            }
        }

        private string _labelParcels;

        public string LabelParcels
        {
            get => _labelParcels;
            set
            {
                _labelParcels = value;
                RaisePropertyChanged(() => LabelParcels);
            }
        }

        private string _labelTotalVat;

        public string LabelTotalVat
        {
            get => _labelTotalVat;
            set
            {
                _labelTotalVat = value;
                RaisePropertyChanged(() => LabelTotalVat);
            }
        }

        private string _labelTotalBrutto;

        public string LabelTotalBrutto
        {
            get => _labelTotalBrutto;
            set
            {
                _labelTotalBrutto = value;
                RaisePropertyChanged(() => LabelTotalBrutto);
            }
        }

        private string _labelPos;

        public string LabelPos
        {
            get => _labelPos;
            set
            {
                _labelPos = value;
                RaisePropertyChanged(() => LabelPos);
            }
        }

        private string _labelCount;

        public string LabelCount
        {
            get => _labelCount;
            set
            {
                _labelCount = value;
                RaisePropertyChanged(() => LabelCount);
            }
        }

        private string _labelContent;

        public string LabelContent
        {
            get => _labelContent;
            set
            {
                _labelContent = value;
                RaisePropertyChanged(() => LabelContent);
            }
        }

        private string _labelAmount;

        public string LabelAmount
        {
            get => _labelAmount;
            set
            {
                _labelAmount = value;
                RaisePropertyChanged(() => LabelAmount);
            }
        }

        private string _labelUnitPrice;

        public string LabelUnitPrice
        {
            get => _labelUnitPrice;
            set
            {
                _labelUnitPrice = value;
                RaisePropertyChanged(() => LabelUnitPrice);
            }
        }

        private string _labelPositionTotal;

        public string LabelPositionTotal
        {
            get => _labelPositionTotal;
            set
            {
                _labelPositionTotal = value;
                RaisePropertyChanged(() => LabelPositionTotal);
            }
        }

        private string _labelArticle;

        public string LabelArticle
        {
            get => _labelArticle;
            set
            {
                _labelArticle = value;
                RaisePropertyChanged(() => LabelArticle);
            }
        }

        private string _labelButtonSearch;

        public string LabelButtonSearch
        {
            get => _labelButtonSearch;
            set
            {
                _labelButtonSearch = value;
                RaisePropertyChanged(() => LabelButtonSearch);
            }
        }

        private string _labelName1;

        public string LabelName1
        {
            get => _labelName1;
            set
            {
                _labelName1 = value;
                RaisePropertyChanged(() => LabelName1);
            }
        }

        private string _labelStock;

        public string LabelStock
        {
            get => _labelStock;
            set
            {
                _labelStock = value;
                RaisePropertyChanged(() => LabelStock);
            }
        }

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
                DocumentService.ChangeDocumentdetailState(EOrderdetailState
                    .NEW); //important must be set BEFORE article!
                DocumentService.Documentdetail.Article = Article;
                RaisePropertyChanged(() => Article);
                Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentDetail));
            }
        }

        private List<Article> _listArticles;

        public List<Article> ListArticles
        {
            get => _listArticles;
            set
            {
                _listArticles = value;
                RaisePropertyChanged(() => ListArticles);
            }
        }

        public Document Document => DocumentService.Document;

        public ObservableCollection<Documentdetail> Documentdetails
        {
            get => DocumentService.Document.Documentdetails;
            set
            {
                DocumentService.Document.Documentdetails = value;
                RaisePropertyChanged(() => Documentdetails);
            }
        }

        public ESettingType KeyboardSetting =>
            (ESettingType) SettingService.Get<int>(ESettingType.KeyboardTypeOnSearch);

        #endregion

        #region ICommand implementations

        private MvxCommand<Documentdetail> _positionEditCommand;

        public ICommand PositionEditCommand
        {
            get
            {
                _positionEditCommand = _positionEditCommand ?? new MvxCommand<Documentdetail>(DoPositionEditCommand);
                return _positionEditCommand;
            }
        }

        private void DoPositionEditCommand(Documentdetail objDocDetail)
        {
            objDocDetail.State = (int) EOrderdetailState.EDIT;
            DocumentService.Documentdetail = objDocDetail;
            Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentDetail));
        }

        private MvxCommand<Documentdetail> _positionDeleteCommand;

        public ICommand PositionDeleteCommand
        {
            get
            {
                _positionDeleteCommand =
                    _positionDeleteCommand ?? new MvxCommand<Documentdetail>(DoPositionDeleteCommand);
                return _positionDeleteCommand;
            }
        }

        private void DoPositionDeleteCommand(Documentdetail objDocDetail)
        {
            DocumentService.Documentdetail = objDocDetail;
            DocumentService.RemoveDocumentdetail();
        }

        private MvxCommand _saveDocumentCommand;

        public ICommand SaveDocumentCommand
        {
            get
            {
                _saveDocumentCommand = _saveDocumentCommand ?? new MvxCommand(DoSaveDocumentCommand);
                return _saveDocumentCommand;
            }
        }

        private void DoSaveDocumentCommand()
        {
            DocumentService.SaveDocument();
        }

        private MvxCommand _discarDocumentCommand;

        public ICommand DiscarDocumentCommand
        {
            get
            {
                _discarDocumentCommand = _discarDocumentCommand ?? new MvxCommand(DoDiscarDocumentCommand);
                return _discarDocumentCommand;
            }
        }

        private void DoDiscarDocumentCommand()
        {
            DocumentService.DiscarDocument();
        }

        #endregion
    }
}
