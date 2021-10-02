using System;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using System.Linq;

namespace pocketseller.core.ViewModels
{
    public class DocumentdetailViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public DocumentdetailViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
        }

        #endregion

        #region Private methods
        private PictureViewModel _pictureViewModel;
        public PictureViewModel PictureViewModel
        {
            get => _pictureViewModel ?? (_pictureViewModel = CMvvmCrossTools.LoadViewModel<PictureViewModel>());
            set => _pictureViewModel = value;
        }

        private bool CheckStockAvailability()
        {
            if (OrderSettings.Instance.CheckStock)
                return TextStockCount * TextContent >= TextAmount;

            return true;
        }

        private bool CheckPriceLowerThanVK(int iPercent)
        {
            return TextPrice <= GetEKPriceWithPercent(iPercent);
        }

        private bool CheckVKHeigherThanEK(int iPercent)
        {
            if(OrderSettings.Instance.CheckVKMustHigher)
                return TextPrice >= GetEKPriceWithPercent(iPercent);

            return true;
        }

        private decimal GetEKPriceWithPercent(int iPercent)
        {
            decimal dEKPrice = 0;

            switch (Mode)
            {
                case CDocumentService.EMode.Order:
                    dEKPrice = DocumentService.Documentdetail.Article.PurchasePrice.Value;
                    break;
                case CDocumentService.EMode.Quotation:
                    dEKPrice = DocumentService.Quotationdetail.Article.PurchasePrice.Value;
                    break;
            }

            var dResult = dEKPrice * (1 + ((decimal)iPercent / 100));
            return dResult;
        }

        private void CloseDetailAndShowOrder()
        {
            Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Exit));
            Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentOrder));
        }

        private void SaveCloseDetailAndShowOrder()
        {
            switch (Mode)
            {
                case CDocumentService.EMode.Order:
                    DocumentService.AddOrUpdateDocumentdetail();
                    break;
                case CDocumentService.EMode.Quotation:
                    DocumentService.AddOrUpdateQuotationdetail();
                    break;

            }

            Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Exit));
            Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentOrder));
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.Article;

            LabelStockCharge = Language.Pos;
            LabelStockDate = Language.Date;
            LabelStockContent = Language.Content;
            LabelStockPackage = Language.Parcel;
            LabelStockEKPrice = Language.Price;

            LabelCancel = Language.Cancel;
            LabelLastPrice = Language.LastPrice;
            LabelLastDate = Language.LastDate;
            LabelLastCount = Language.LastCount;
            
            LabelStockCount = Language.CurrentStockCount;
            LabelStockAmount = Language.CurrentStockAmount;
            LabelContent = Language.Content;
            LabelCount = Language.Count;
            LabelAmount = Language.Amount;
            LabelPrice = Language.Price;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelStockCharge;
        public string LabelStockCharge { get => _labelStockCharge;
            set { _labelStockCharge = value; RaisePropertyChanged(() => LabelStockCharge); } }

        private string _labelStockDate;
        public string LabelStockDate { get => _labelStockDate;
            set { _labelStockDate = value; RaisePropertyChanged(() => LabelStockDate); } }

        private string _labelStockContent;
        public string LabelStockContent { get => _labelStockContent;
            set { _labelStockContent = value; RaisePropertyChanged(() => LabelStockContent); } }

        private string _labelStockPackage;
        public string LabelStockPackage { get => _labelStockPackage;
            set { _labelStockPackage = value; RaisePropertyChanged(() => LabelStockPackage); } }

        private string _labelStockEKPrice;
        public string LabelStockEKPrice { get => _labelStockEKPrice;
            set { _labelStockEKPrice = value; RaisePropertyChanged(() => LabelStockEKPrice); } }

        private string _labelCancel;
        public string LabelCancel { get => _labelCancel;
            set { _labelCancel = value; RaisePropertyChanged(() => LabelCancel); } }

        private string _labelLastPrice;
        public string LabelLastPrice { get => _labelLastPrice;
            set { _labelLastPrice = value; RaisePropertyChanged(() => LabelLastPrice); } }
   
        private string _labelLastDate;
        public string LabelLastDate { get => _labelLastDate;
            set { _labelLastDate = value; RaisePropertyChanged(() => LabelLastDate); } }

        private string _labelLastCount;
        public string LabelLastCount { get => _labelLastCount;
            set { _labelLastCount = value; RaisePropertyChanged(() => LabelLastCount); } }

        private string _labelStockCount;
        public string LabelStockCount { get => _labelStockCount;
            set { _labelStockCount = value; RaisePropertyChanged(() => LabelStockCount); } }

        private string _labelStockAmount;
        public string LabelStockAmount { get => _labelStockAmount;
            set { _labelStockAmount = value; RaisePropertyChanged(() => LabelStockAmount); } }

        private string _labelContent;
        public string LabelContent { get => _labelContent;
            set { _labelContent = value; RaisePropertyChanged(() => LabelContent); } }

        private string _labelCount;
        public string LabelCount { get => _labelCount;
            set { _labelCount = value; RaisePropertyChanged(() => LabelCount); } }

        private string _labelAmount;
        public string LabelAmount { get => _labelAmount;
            set { _labelAmount = value; RaisePropertyChanged(() => LabelAmount); } }
        
        private string _labelPrice;
        public string LabelPrice { get => _labelPrice;
            set { _labelPrice = value; RaisePropertyChanged(() => LabelPrice); } }
        public string Headline => string.Format("{0} {1}",  TextArticlenumber, TextName);

        public Article Article
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        if (DocumentService.Quotation.Quotationdetails == null || DocumentService.Quotation.Quotationdetails.Count == 0)
                            DocumentService.Quotation.Quotationdetails = new System.Collections.ObjectModel.ObservableCollection<Models.Quotationdetail>(GetAllValidQuotations());
                        return DocumentService.Documentdetail.Article;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article;
                    default:
                        return null;
                }
            }
        }

        public string ImageUrl => Article.ImageUrl(96);
        public string ImageUrlDefault => Article.ImageUrl(96, "default");
        public string ImageUrlError => Article.ImageUrl(96, "error");
        public string TextArticlenumber => Article != null ? Article.Articlenumber : "";

        public string TextName
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.Documentdetail.Article.Name1
                            : "";
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.Quotationdetail.Article.Name1
                            : "";
                    default:
                        return string.Empty;
                }
            }
        }

        public decimal TextContent
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Content;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Content;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        DocumentService.Documentdetail.Content = value;
                        RaisePropertyChanged(() => TextContent);
                        break;
                    case CDocumentService.EMode.Quotation:
                        DocumentService.Quotationdetail.Content = value;
                        RaisePropertyChanged(() => TextContent);
                        break;
                }
            }
        }

        public decimal TextCount
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Count;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Count;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        DocumentService.Documentdetail.Count = value;
                        RaisePropertyChanged(() => TextCount);
                        break;
                    case CDocumentService.EMode.Quotation:
                        DocumentService.Quotationdetail.Count = value;
                        RaisePropertyChanged(() => TextCount);
                        break;
                }
            }
        }

        public decimal TextAmount
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Amount;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Amount;
                    default:
                return 0;
                }
            }
            set
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        DocumentService.Documentdetail.Amount = value;
                        RaisePropertyChanged(() => TextAmount);
                        break;
                    case CDocumentService.EMode.Quotation:
                        DocumentService.Quotationdetail.Amount = value;
                        RaisePropertyChanged(() => TextAmount);
                        break;
                }
            }
        }

        public decimal TextPrice
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Nettoprice;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Nettoprice;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        var nettoPrice = value;
                        var offerArticle = DocumentService.Quotation.Quotationdetails.FirstOrDefault(q => q.ArticleNr.Equals(Article.Articlenumber));
                        if(offerArticle != null)
                            nettoPrice = offerArticle.Nettoprice;
                        DocumentService.Documentdetail.Nettoprice = nettoPrice;
                        RaisePropertyChanged(() => TextPrice);
                        break;
                    case CDocumentService.EMode.Quotation:
                        DocumentService.Quotationdetail.Nettoprice = value;
                        RaisePropertyChanged(() => TextPrice);
                        break;
                }
            }
        }

        public decimal TextStockCount
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.GetLocalAvailableCount(DocumentService.Documentdetail.Article)
                            : 0;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.GetLocalAvailableCount(DocumentService.Quotationdetail.Article)
                            : 0;
                    default:
                        return 0;
                }
            }
        }

        public decimal TextStockAmount
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.GetLocalAvailableAmount(DocumentService.Documentdetail.Article)
                            : 0;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.GetLocalAvailableAmount(DocumentService.Quotationdetail.Article)
                            : 0;
                    default:
                        return 0;
                }
            }
        }

        public decimal TextLastPrice => DocumentService.Lastprice.Price;

        public DateTime TextLastDate => DocumentService.Lastprice.Salesdate;

        public decimal TextLastCount => DocumentService.Lastprice.Count;

        public decimal TextLastContent => DocumentService.Lastprice.Content;

        public decimal TextPrice0
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.Documentdetail.Article.ArticlepricesAt(0).Price
                            : 0;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.Quotationdetail.Article.ArticlepricesAt(0).Price
                            : 0;
                    default:
                        return 0;
                }
            }
        }

        public decimal TextPrice1
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.Documentdetail.Article.ArticlepricesAt(1).Price
                            : 0;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.Quotationdetail.Article.ArticlepricesAt(1).Price
                            : 0;
                    default:
                        return 0;
                }
            }
        }

        public decimal TextPrice2
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? DocumentService.Documentdetail.Article.ArticlepricesAt(2).Price
                            : 0;
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? DocumentService.Quotationdetail.Article.ArticlepricesAt(2).Price
                            : 0;
                    default:
                        return 0;
                }
            }
        }

        public string LabelPriceGroup1
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? "Preis 1"
                            : "";
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? "Preis 1"
                            : "";
                    default:
                        return string.Empty;
                }
            }
        }

        public string LabelPriceGroup2
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? "Preis 2"
                            : "";
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? "Preis 2"
                            : "";
                    default:
                        return string.Empty;
                }
            }
        }

        public string LabelPriceGroup3
        {
            get
            {
                switch (Mode)
                {
                    case CDocumentService.EMode.Order:
                        return DocumentService.Documentdetail.Article != null
                            ? "Preis 3"
                            : "";
                    case CDocumentService.EMode.Quotation:
                        return DocumentService.Quotationdetail.Article != null
                            ? "Preis 3"
                            : "";
                    default:
                        return string.Empty;
                }
            }
        }

        public ESettingType KeyboardSetting => (ESettingType)SettingService.Get<int>(ESettingType.KeyboardTypeInDocumentdetail);

        public CDocumentService.EMode Mode
        {
            get => DocumentService.Mode;
            set => DocumentService.Mode = value;
        }

        #endregion

        #region ICommand implementations

        private MvxCommand _addDocumentdetailCommand;
        public ICommand AddDocumentdetailCommand { get { _addDocumentdetailCommand = _addDocumentdetailCommand ?? new MvxCommand(DoAddDocumentdetailCommand); return _addDocumentdetailCommand; } }
        private void DoAddDocumentdetailCommand()
        {
            if (!CheckStockAvailability())
            {
                var objConfirmConfig = new ConfirmConfig
                {
                    Title = Language.Attention,
                    Message = Language.InfoStockNotAvailable,
                    OkText = Language.Yes,
                    CancelText = Language.No,
                    OnAction = bResult => { if (bResult) SaveCloseDetailAndShowOrder(); }
                };

                Mvx.IoCProvider.Resolve<IUserDialogs>().Confirm(objConfirmConfig);
                return;
            }

            if(Mode == CDocumentService.EMode.Order)
            {

                var isNormalOrder = DocumentService.Documentdetail.Amount > 0;
                var lowestVkPrice = DocumentService.Documentdetail.Article.Articleprices.FirstOrDefault(a => a.PriceGroupNr == 3)?.Price;
                var currentVkPrice = TextPrice;

                var offerArticle = DocumentService.Quotation.Quotationdetails.FirstOrDefault(q => q.ArticleNr.Equals(Article.Articlenumber));
                if(offerArticle != null)
                {
                    lowestVkPrice = offerArticle.Nettoprice;
                }

                if (lowestVkPrice == null)
                {
                    var alertService = Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync("Verkaufspreis darf nicht 0 sein", Language.Attention);
                    Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Stay));
                    return;
                }

                if (currentVkPrice == 0)
                {
                    var alertService = Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync("Verkaufspreis darf nicht 0 sein", Language.Attention);
                    Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Stay));
                    return;
                }

                //if (currentVkPrice < lowestVkPrice)
                //{
                //    var alertService = Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync("Verkaufspreis darf nicht unter Preisgruppe 3 sein", Language.Attention);
                //    Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Stay));
                //    return;
                //}

                if (lowestVkPrice > 0 && lowestVkPrice < TextPrice)
                {
                    var iVKHeigherPercent = OrderSettings.Instance.VKMustHigherPercent;
                    if (!CheckVKHeigherThanEK(iVKHeigherPercent))
                    {
                        Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.InfoPriceMustGreaterThanEK, Language.Attention);
                        Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Stay));
                        return;
                    }
                }

                var iVKLowerPercent = OrderSettings.Instance.VKMustLowerPercent;
                if (!CheckPriceLowerThanVK(iVKLowerPercent))
                {
                    Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.InfoPriceMustInRange, Language.Attention);
                    Messenger.Publish(new DocumentdetailMessage(this, EDocumentdetailAction.Stay));
                    return;
                }
            }


            switch (Mode)
            {
                case CDocumentService.EMode.Order:
                    if (DocumentService.ArticleAlreadyExistsInDocument() && DocumentService.Documentdetail.State == (int)EOrderdetailState.NEW)
                    {
                        var objConfirmConfig = new ConfirmConfig
                        {
                            Title = Language.Attention,
                            Message = Language.ArticleAlreadyExists,
                            OkText = Language.Yes,
                            CancelText = Language.No,
                            OnAction = bResult => { if (bResult) SaveCloseDetailAndShowOrder(); }
                        };

                        Mvx.IoCProvider.Resolve<IUserDialogs>().Confirm(objConfirmConfig);

                        CloseDetailAndShowOrder();
                        return;
                    }
                    break;
                case CDocumentService.EMode.Quotation:
                    if (DocumentService.ArticleAlreadyExistsInQuotation() && DocumentService.Quotationdetail.State == (int)EOrderdetailState.NEW)
                    {
                        var objConfirmConfig = new ConfirmConfig
                        {
                            Title = Language.Attention,
                            Message = Language.ArticleAlreadyExists,
                            OkText = Language.Yes,
                            CancelText = Language.No,
                            OnAction = bResult => { if (bResult) SaveCloseDetailAndShowOrder(); }
                        };

                        Mvx.IoCProvider.Resolve<IUserDialogs>().Confirm(objConfirmConfig);

                        CloseDetailAndShowOrder();
                        return;
                    }
                    break;
            }

            //if all conditions matched to the expectations then close documentdetail, add this documentdetail to order and show the order
            SaveCloseDetailAndShowOrder();
        }

        private MvxCommand _cancelDocumentdetailCommand;
        public ICommand CancelDocumentdetailCommand { get { _cancelDocumentdetailCommand = _cancelDocumentdetailCommand ?? new MvxCommand(DoCancelDocumentdetailCommand); return _cancelDocumentdetailCommand; } }
        private void DoCancelDocumentdetailCommand()
        {
        }

        private MvxCommand<decimal> _setCountCommand;
        public ICommand SetCoundCommand { get { _setCountCommand = _setCountCommand ?? new MvxCommand<decimal>(DoSetCoundCommand); return _setCountCommand; } }
        private void DoSetCoundCommand(decimal dValue)
        {
            TextCount = dValue;
        }

        private MvxCommand<decimal> _SetAmountCommand;
        public ICommand SetAmountCommand { get { _SetAmountCommand = _SetAmountCommand ?? new MvxCommand<decimal>(DoSetAmountCommand); return _SetAmountCommand; } }
        private void DoSetAmountCommand(decimal dValue)
        {
            TextAmount = dValue;
        }

        private MvxCommand<decimal> _SetPriceCommand;
        public ICommand SetPriceCommand { get { _SetPriceCommand = _SetPriceCommand ?? new MvxCommand<decimal>(DoSetPriceCommand); return _SetPriceCommand; } }
        private void DoSetPriceCommand(decimal dValue)
        {
            TextPrice = dValue;
        }

        #endregion
    }

    public class ValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
