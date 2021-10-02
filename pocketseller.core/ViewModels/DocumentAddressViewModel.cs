using System.Collections.Generic;
using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentAddressViewModel : BaseViewModel
    {
        #region Private properties

        private readonly string STATE = "state";

        #endregion

        #region Constructors

        public DocumentAddressViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
        }

        #endregion

        #region Private methods

        private void SearchNow()
        {
            if (SearchKey.Length > 0 && SearchKey.Length >= SettingService.Get<int>(ESettingType.SearchMaxChar) && SearchKey.Length < 20)
            {
                //TODO: workaround when state=0 and normally should be state=1, why? > analyze, fix and remove this workaround!
                string strCommand = string.Format("{0}=", STATE);
                if(SearchKey.ToLower().StartsWith(strCommand))
                {
                    string strValue = SearchKey.Substring(strCommand.Length, 1);
                    int iValue = -1;
                    if (int.TryParse(strValue, out iValue))
                    {
                        DocumentService.Document.EditMode = false;
                        DocumentService.Document.ChangetState((EOrderState)iValue);
                        Document.ChangeState(DocumentService.Document, (EOrderState)iValue);

                        Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(
                            string.Format("State is changed to: {0}", iValue),
                            null,
                            Language.Info);
                    }
                }
                else
                {
                    ListAddresses = Adress.Find(SearchKey);                    
                }
            }
            else
            {
                ListAddresses = new List<Adress>();                
            }
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.TabAddress;
            LabelAddressInfo = Language.AddressInfo;
            LabelAddressnumber = Language.Addressnumber;
            LabelName1 = Language.Name1;
            LabelName2 = Language.Name2;
            LabelStreet = Language.Street;
            LabelZip = Language.Zip;
            LabelCity = Language.City;
            LabelPhone1 = Language.Phone1;
            LabelPhone2 = Language.Phone2;
            LabelFax = Language.Fax;
            LabelHint = Language.SearchAddress;

            LabelAdressnumber = Language.Number;
            ListAddresses = new List<Adress>();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties
        
        private string _labelHint;
        public string LabelHint 
        {
            get => _labelHint;
            set 
            {
                if(!string.IsNullOrEmpty(_labelHint))
                    return;

                _labelHint = value; RaisePropertyChanged(() => LabelHint); 
            }
        }

        private string _labelAddressInfo;
        public string LabelAddressInfo { get => _labelAddressInfo;
            set { _labelAddressInfo = value; RaisePropertyChanged(() => LabelAddressInfo); } }

        private string _labelAddressnumber;
        public string LabelAddressnumber { get => _labelAddressnumber;
            set { _labelAddressnumber = value; RaisePropertyChanged(() => LabelAddressnumber); } }

        private string _labelName1;
        public string LabelName1 { get => _labelName1;
            set { _labelName1 = value; RaisePropertyChanged(() => LabelName1); } }

        private string _labelName2;
        public string LabelName2 { get => _labelName2;
            set { _labelName2 = value; RaisePropertyChanged(() => LabelName2); } }

        private string _labelStreet;
        public string LabelStreet { get => _labelStreet;
            set { _labelStreet = value; RaisePropertyChanged(() => LabelStreet); } }

        private string _labelZip;
        public string LabelZip { get => _labelZip;
            set { _labelZip = value; RaisePropertyChanged(() => LabelZip); } }

        private string _labelFax;
        public string LabelFax { get => _labelFax;
            set { _labelFax = value; RaisePropertyChanged(() => LabelFax); } }

        private string _labelMobile;
        public string LabelMobile { get => _labelMobile;
            set { _labelMobile = value; RaisePropertyChanged(() => LabelMobile); } }

        private string _labelPhone1;
        public string LabelPhone1 { get => _labelPhone1;
            set { _labelPhone1 = value; RaisePropertyChanged(() => LabelPhone1); } }

        private string _labelPhone2;
        public string LabelPhone2 { get => _labelPhone2;
            set { _labelPhone2 = value; RaisePropertyChanged(() => LabelPhone2); } }

        private string _labelCity;
        public string LabelCity { get => _labelCity;
            set { _labelCity = value; RaisePropertyChanged(() => LabelCity); } }

        private string _labelSearch;
        public string LabelSearch { get => _labelSearch;
            set { _labelSearch = value; RaisePropertyChanged(() => LabelSearch); } }

        private string _labelButtonSearch;
        public string LabelButtonSearch { get => _labelButtonSearch;
            set { _labelButtonSearch = value; RaisePropertyChanged(() => LabelButtonSearch); } }

        private string _labelAdressnumber;
        public string LabelAdressnumber { get => _labelAdressnumber;
            set { _labelAdressnumber = value; RaisePropertyChanged(() => LabelAdressnumber); } }

        private string _searchKey;
        public string SearchKey
        {
            get => _searchKey;
            set
            {
                if (typeof (Adress).ToString() != value)
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

        public Adress Address
        {
            get => DocumentService.Document.Adress;
            set
            {
                DocumentService.Document.Adress = value;
                RaisePropertyChanged(() => Address);
                Messenger.Publish(new DocumentMessage(this, EDocumentAction.ShowDocumentOrder));

                if (!OpenPayment.IsWithinPaymentDays(value))
                {
                    Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(
                        Language.OrderNotSavable,
                        Language.Attention,
                        Language.Ok);
                }
            }
        }

        private List<Adress> _listAddresses;
        public List<Adress> ListAddresses { get => _listAddresses;
            set { _listAddresses = value; RaisePropertyChanged(() => ListAddresses); } }

        public ESettingType KeyboardSetting => (ESettingType)SettingService.Get<int>(ESettingType.KeyboardTypeOnSearch);

        #endregion

        #region ICommand implementations
        #endregion
    }
}
