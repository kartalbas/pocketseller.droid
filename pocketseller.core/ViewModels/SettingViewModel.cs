using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public SettingViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService LanguageServiceService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, LanguageServiceService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
            OrderSettings.Instance.CheckStock = true;
            OrderSettings.Instance.CheckVKMustHigher = true;
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
            OpManagers = EMails.Instance.GetMails();

            LabelTitle = Language.Settings;
            LabelUserSetting = Language.UserSettings;
            LabelDataFlow = Language.Dataflow;

            LabelOpManager = Language.OpManager;
            LabelLanguage = Language.Langage;
            LabelGerman = Language.German;
            LabelTurkish = Language.Turkish;
            LabelEnglish = Language.English;

            LabelAddressSearchType = Language.AddressSearchType;
            LabelArticleSearchType = Language.ArticleSearchType;
            LabelBeginningOfWord = Language.BeginningOfWord;
            LabelOverallInWord = Language.OverallInWord;

            LabelPriceSelect = Language.PriceSelect;

            LabelOrderNumberBegin = Language.OrderNumberBegin;
            LabelOrderNumberCurrent = Language.OrderNumberCurrent;
            LabelOrderSearchMaxChar = Language.OrderSearchMaxChar;
            LabelOrderMaxDocumentdetails = Language.MaxDocumentDetails;

            LabelCheckEK = Language.CheckEK;
            LabelCheckStock = Language.CheckStock;
            LabelGreaterEKInPercent = Language.GreaterEKInPercent;
            LabelLowerInputInPercent = Language.LowerInputInPercent;

            LabelNo = Language.No;
            LabelYes = Language.Yes;

            LabelKeyboardTypeInDocumentdetail = Language.KeyboardInDocumentdetail;
            LabelKeyboardTypeOnSearch = Language.KeyboardOnSearch;
            LabelSearchType = Language.SearchType;
            LabelSearchTypeNormal = Language.NormalSearchType;
            LabelSearchTypeUpper = Language.UpperCaseSearchType;
            LabelSearchTypeLower = Language.LowerCaseSearchType;

            LabelActivationCode = Language.ActivationCode;

            LabelPictureUrl = Language.PictureUrl;

            LabelUpdateOrderTemplate = Language.UpdateExcelTemplate;
            LabelBackupDatabase = Language.BackupDatabase;
            LabelRestoreDatabase = Language.RestoreDatabase;

            LabelCashAndCarry = Language.CashAndCarry;

            IsEnabledOrderNumberBegin = true;
            IsEnabledOrderNumberCurrent = true;
            IsEnabledOrderSearchMaxChar = true;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public Properties for handling settings

        public ObservableCollection<string> ListYesNo => new ObservableCollection<string> { LabelNo, LabelYes };

        public ObservableCollection<string> ListKeyboardType => new ObservableCollection<string> { Language.NormalKeyboard, Language.PhoneKeyboard, Language.NumberKeyboard };

        private ObservableCollection<string> _opManagers;
        public ObservableCollection<string> OpManagers
        {
            get => _opManagers;
            set
            {
                _opManagers = value;
                RaisePropertyChanged(() => OpManagers);
            }
        }

        private string _labelOpManager;
        public string LabelOpManager
        {
            get => _labelOpManager;
            set { _labelOpManager = value; RaisePropertyChanged(() => LabelOpManager); }
        }

        public string SelectedItemOpManager
        {
            get => SettingService.Get<string>(ESettingType.OpManager);
            set
            {
                SettingService.Set(ESettingType.OpManager, value);
                RaisePropertyChanged(() => SelectedItemOpManager);
            }
        }


        private string _labelCashAndCarry;
        public string LabelCashAndCarry
        {
            get => _labelCashAndCarry;
            set { _labelCashAndCarry = value; RaisePropertyChanged(() => LabelCashAndCarry); }
        }

        private string _labelRestoreDatabase;
        public string LabelRestoreDatabase { get => _labelRestoreDatabase;
            set { _labelRestoreDatabase = value; RaisePropertyChanged(() => LabelRestoreDatabase); } }

        private string _labelBackupDatabase;
        public string LabelBackupDatabase { get => _labelBackupDatabase;
            set { _labelBackupDatabase = value; RaisePropertyChanged(() => LabelBackupDatabase); } }

        private string _labelUpdateOrderTemplate;
        public string LabelUpdateOrderTemplate { get => _labelUpdateOrderTemplate;
            set { _labelUpdateOrderTemplate = value; RaisePropertyChanged(() => LabelUpdateOrderTemplate); } }

        private string _labelYes;
        public string LabelYes { get => _labelYes;
            set { _labelYes = value; RaisePropertyChanged(() => LabelYes); } }

        private string _labelSearchTypeNormal;
        public string LabelSearchTypeNormal { get => _labelSearchTypeNormal;
            set { _labelSearchTypeNormal = value; RaisePropertyChanged(() => LabelSearchTypeNormal); } }

        private string _labelSearchTypeUpper;
        public string LabelSearchTypeUpper { get => _labelSearchTypeUpper;
            set { _labelSearchTypeUpper = value; RaisePropertyChanged(() => LabelSearchTypeUpper); } }

        private string _labelSearchTypeLower;
        public string LabelSearchTypeLower { get => _labelSearchTypeLower;
            set { _labelSearchTypeLower = value; RaisePropertyChanged(() => LabelSearchTypeLower); } }

        private string _labelUserSetting = "";
        public string LabelUserSetting
        {
            get => _labelUserSetting;
            set { _labelUserSetting = value; RaisePropertyChanged(() => LabelUserSetting); }
        }

        private string _labelDataFlow = "";
        public string LabelDataFlow
        {
            get => _labelDataFlow;
            set { _labelDataFlow = value; RaisePropertyChanged(() => LabelDataFlow); }
        }

        public int SelectedIndexLanguage
        {
            get => (int)SettingService.Get<ELanguage>(ESettingType.Language);
            set
            {
                SettingService.Set(ESettingType.Language, LanguageService.ConvertLanguage(value));
                RaisePropertyChanged(() => SelectedIndexLanguage);
                LanguageService.ChangeLanguageThroughSettings();
            }
        }
        
        private string _labelLanguage = "";
        public string LabelLanguage
        {
            get => _labelLanguage;
            set { _labelLanguage = value; RaisePropertyChanged(() => LabelLanguage); }
        }

        private string _labelGerman = "";
        public string LabelGerman
        {
            get => _labelGerman;
            set { _labelGerman = value; RaisePropertyChanged(() => LabelGerman); }
        }

        private string _labelTurkish = "";
        public string LabelTurkish
        {
            get => _labelTurkish;
            set { _labelTurkish = value; RaisePropertyChanged(() => LabelTurkish); }
        }

        private string _labelEnglish = "";
        public string LabelEnglish
        {
            get => _labelEnglish;
            set { _labelEnglish = value; RaisePropertyChanged(() => LabelEnglish); }
        }

        private string _labelNo = "";
        public string LabelNo
        {
            get => _labelNo;
            set { _labelNo = value; RaisePropertyChanged(() => LabelNo); }
        }

        public List<string> Languages => new List<string> { LabelTurkish, LabelEnglish, LabelGerman };

        public int SelectedIndexArticleSearchType
        {
            get => SettingService.Get<int>(ESettingType.SearchTypeArticle);
            set
            {
                RaisePropertyChanged(() => SelectedIndexArticleSearchType);
                SettingService.Set(ESettingType.SearchTypeArticle, value);
            }
        }

        private string _labelArticleSearchType = "";
        public string LabelArticleSearchType
        {
            get => _labelArticleSearchType;
            set { _labelArticleSearchType = value; RaisePropertyChanged(() => LabelArticleSearchType); }
        }

        private string _labelBeginningOfWord = "";
        public string LabelBeginningOfWord
        {
            get => _labelBeginningOfWord;
            set { _labelBeginningOfWord = value; RaisePropertyChanged(() => LabelBeginningOfWord); }
        }

        private string _labelOverallInWord = "";
        public string LabelOverallInWord
        {
            get => _labelOverallInWord;
            set { _labelOverallInWord = value; RaisePropertyChanged(() => LabelOverallInWord); }
        }

        private string _labelOrderMaxDocumentdetails;
        public string LabelOrderMaxDocumentdetails { get => _labelOrderMaxDocumentdetails;
            set { _labelOrderMaxDocumentdetails = value; RaisePropertyChanged(() => LabelOrderMaxDocumentdetails); } }

        public int TextOrderMaxDocumentdetails
        {
            get => OrderSettings.Instance.MaxDocumentdetails;
            set
            {
                OrderSettings.Instance.MaxDocumentdetails = value;
                RaisePropertyChanged(() => TextOrderMaxDocumentdetails);
            }
        }

        private string _labelSearchType;
        public string LabelSearchType { get => _labelSearchType;
            set { _labelSearchType = value; RaisePropertyChanged(() => LabelSearchType); } }

        public ObservableCollection<string> SearchingTypes => new ObservableCollection<string> { LabelSearchTypeNormal, LabelSearchTypeLower, LabelSearchTypeUpper };

        private string _labelKeyboardTypeOnSearch;
        public string LabelKeyboardTypeOnSearch { get => _labelKeyboardTypeOnSearch;
            set { _labelKeyboardTypeOnSearch = value; RaisePropertyChanged(() => LabelKeyboardTypeOnSearch); } }

        public int SelectedIndexKeyboardOnSearch
        {
            get => SettingService.Get<int>(ESettingType.KeyboardTypeOnSearch);
            set
            {
                SettingService.Set(ESettingType.KeyboardTypeOnSearch, value);
                RaisePropertyChanged(() => SelectedIndexKeyboardOnSearch);
            }
        }

        public int SelectedIndexCashAndCarry
        {
            get => SettingService.Get<int>(ESettingType.CashAndCarry);
            set
            {
                SettingService.Set(ESettingType.CashAndCarry, value);
                RaisePropertyChanged(() => SelectedIndexCashAndCarry);
            }
        }

        private string _labelKeyboardTypeInDocumentdetail;
        public string LabelKeyboardTypeInDocumentdetail { get => _labelKeyboardTypeInDocumentdetail;
            set { _labelKeyboardTypeInDocumentdetail = value; RaisePropertyChanged(() => LabelKeyboardTypeInDocumentdetail); } }

        public int SelectedIndexKeyboardTypeInDocumentdetail
        {
            get => SettingService.Get<int>(ESettingType.KeyboardTypeInDocumentdetail);
            set
            {
                SettingService.Set(ESettingType.KeyboardTypeInDocumentdetail, value);
                RaisePropertyChanged(() => SelectedIndexKeyboardTypeInDocumentdetail);
            }
        }

        public int SelectedIndexSearchType
        {
            get => SettingService.Get<int>(ESettingType.SearchType);
            set
            {
                SettingService.Set(ESettingType.SearchType, value);
                RaisePropertyChanged(() => SelectedIndexSearchType);
            }
        }

        public int SelectedIndexAddressSearchType
        {
            get => SettingService.Get<int>(ESettingType.SearchTypeAddress);
            set
            {
                SettingService.Set(ESettingType.SearchTypeAddress, value);
                RaisePropertyChanged(() => SelectedIndexAddressSearchType);
            }
        }

        private string _labelAddressSearchType = "";
        public string LabelAddressSearchType
        {
            get => _labelAddressSearchType;
            set { _labelAddressSearchType = value; RaisePropertyChanged(() => LabelAddressSearchType); }
        }

        private string _labelActivationCode;
        public string LabelActivationCode { get => _labelActivationCode;
            set { _labelActivationCode = value; RaisePropertyChanged(() => LabelActivationCode); } }

        public int SelectedIndexPriceSelect
        {
            get => OrderSettings.Instance.AutoPrice;
            set
            {
                OrderSettings.Instance.AutoPrice = value;
                RaisePropertyChanged(() => SelectedIndexPriceSelect);
            }
        }

        public ObservableCollection<string> SearchTypes => new ObservableCollection<string> { LabelBeginningOfWord, LabelOverallInWord };

        private string _labelPriceSelect = "";
        public string LabelPriceSelect
        {
            get => _labelPriceSelect;
            set { _labelPriceSelect = value; RaisePropertyChanged(() => LabelPriceSelect); }
        }

        public List<string> Pricegroups
        {
            get
            {
                var priceGroups = DataService.PocketsellerConnection.Table<Articleprice>()
                    .Select(p => p.PriceGroupNr.ToString())
                    .Distinct()
                    .OrderBy(p => Convert.ToInt32(p))
                    .ToList();

                return priceGroups;
            }
        }

        private string _labelOrderNumberBegin = "";
        public string LabelOrderNumberBegin
        {
            get => _labelOrderNumberBegin;
            set { _labelOrderNumberBegin = value; RaisePropertyChanged(() => LabelOrderNumberBegin); }
        }

        public int TextOrderNumberBegin
        {
            get => OrderSettings.Instance.DefaultDocNr;
            set 
            {
                OrderSettings.Instance.DefaultDocNr = value;
                RaisePropertyChanged(() => TextOrderNumberBegin);
            }
        }

        private string _labelOrderNumberCurrent = "";
        public string LabelOrderNumberCurrent
        {
            get => _labelOrderNumberCurrent;
            set { _labelOrderNumberCurrent = value; RaisePropertyChanged(() => LabelOrderNumberCurrent); }
        }

        public int TextOrderNumberCurrent
        {
            get => OrderSettings.Instance.CurrentDocNr;
            set
            {
                OrderSettings.Instance.CurrentDocNr = value;
                RaisePropertyChanged(() => TextOrderNumberCurrent);                    
            }
        }

        private string _labelOrderSearchMaxChar = "";
        public string LabelOrderSearchMaxChar
        {
            get => _labelOrderSearchMaxChar;
            set { _labelOrderSearchMaxChar = value; RaisePropertyChanged(() => LabelOrderSearchMaxChar); }
        }

        public int TextOrderSearchMaxChar
        {
            get => SettingService.Get<int>(ESettingType.SearchMaxChar);
            set 
            { 
                SettingService.Set(ESettingType.SearchMaxChar, value);
                RaisePropertyChanged(() => TextOrderSearchMaxChar);
            }
        }

        private bool _isEnabledOrderNumberBegin;
        public bool IsEnabledOrderNumberBegin
        {
            get => _isEnabledOrderNumberBegin;
            set { _isEnabledOrderNumberBegin = value; RaisePropertyChanged(() => IsEnabledOrderNumberBegin); }
        }

        private bool _isEnabledOrderNumberCurrent;
        public bool IsEnabledOrderNumberCurrent
        {
            get => _isEnabledOrderNumberCurrent;
            set { _isEnabledOrderNumberCurrent = value; RaisePropertyChanged(() => IsEnabledOrderNumberCurrent); }
        }

        private bool _isEnabledOrderSearchMaxChar;
        public bool IsEnabledOrderSearchMaxChar
        {
            get => _isEnabledOrderSearchMaxChar;
            set { _isEnabledOrderSearchMaxChar = value; RaisePropertyChanged(() => IsEnabledOrderSearchMaxChar); }
        }

        private string _labelCheckStock;
        public string LabelCheckStock { get => _labelCheckStock;
            set { _labelCheckStock = value; RaisePropertyChanged(() => LabelCheckStock); } }
        public int SelectedIndexCheckStock
        {
            get => Convert.ToInt32(OrderSettings.Instance.CheckStock);
            set
            {
                OrderSettings.Instance.CheckStock = Convert.ToBoolean(value);
                RaisePropertyChanged(() => SelectedIndexCheckStock);
            }
        }

        private string _labelCheckEK;
        public string LabelCheckEK { get => _labelCheckEK;
            set { _labelCheckEK = value; RaisePropertyChanged(() => LabelCheckEK); } }
        public int SelectedIndexCheckEK
        {
            get => Convert.ToInt32(OrderSettings.Instance.CheckVKMustHigher);
            set
            {
                OrderSettings.Instance.CheckVKMustHigher = Convert.ToBoolean(value);
                RaisePropertyChanged(() => SelectedIndexCheckEK);
            }
        }

        private string _labelGreaterEKInPercent;
        public string LabelGreaterEKInPercent { get => _labelGreaterEKInPercent;
            set { _labelGreaterEKInPercent = value; RaisePropertyChanged(() => LabelGreaterEKInPercent); } }
        public int TextGreaterEKInPercent
        {
            get => OrderSettings.Instance.VKMustHigherPercent;
            set
            {
                OrderSettings.Instance.VKMustHigherPercent = value;
                RaisePropertyChanged(() => TextGreaterEKInPercent);
            }
        }

        private string _labelLowerInputInPercent;
        public string LabelLowerInputInPercent { get => _labelLowerInputInPercent;
            set { _labelLowerInputInPercent = value; RaisePropertyChanged(() => LabelLowerInputInPercent); } }
        public int TextLowerInputInPercent
        {
            get => OrderSettings.Instance.VKMustLowerPercent;
            set
            {
                OrderSettings.Instance.VKMustLowerPercent = value;
                RaisePropertyChanged(() => TextLowerInputInPercent);
            }
        }

        private string _labelPictureUrl;
        public string LabelPictureUrl { get => _labelPictureUrl;
            set { _labelPictureUrl = value; RaisePropertyChanged(() => LabelPictureUrl); } }
        public string TextPictureUrl
        {
            get =>
                string.IsNullOrEmpty(OrderSettings.Instance.PictureUrl) 
                    ? "http://" 
                    : OrderSettings.Instance.PictureUrl;
            set
            {
                OrderSettings.Instance.PictureUrl = value;
                RaisePropertyChanged(() => TextPictureUrl);
            }
        }

        private MvxCommand<bool> _allDbConnectionsCommand;
        public ICommand AllDbConnectionsCommand { get { _allDbConnectionsCommand = _allDbConnectionsCommand ?? new MvxCommand<bool>(DoAllDbConncectionsCommand); return _allDbConnectionsCommand; } }
        private void DoAllDbConncectionsCommand(bool bEnableDisable)
        {
            if (bEnableDisable)
            {
                DataService.CreateSettingDb();
                var objCurrentSource = Source.Instance.GetCurrentSource();
                DataService.CreatePocketsellerDb(objCurrentSource.DbName);
            }
            else
            {
                if (DataService.PocketsellerConnection != null)
                    DataService.PocketsellerConnection.Close();

                if (DataService.SettingsConnection != null)
                    DataService.SettingsConnection.Close();
            }
        }

        #endregion
    }
}
