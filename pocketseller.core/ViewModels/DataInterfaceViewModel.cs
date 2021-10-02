using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DataInterfaceViewModel : BaseViewModel
    {
        #region Private properties

        #endregion

        #region Constructors

        public DataInterfaceViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            SubscriptionToken1 = Messenger.Subscribe<LanguageServiceMessage>(OnLanguageChanged);
            SubscriptionToken2 = Messenger.Subscribe<SourceViewMessage>(OnSourceChanged);
            LogTag = GetType().Name;
        }

        #endregion

        #region Private methods

        private void OnSourceChanged(SourceViewMessage objMessage)
        {
            if (objMessage.ESourceViewAction == ESourceViewAction.Added || objMessage.ESourceViewAction == ESourceViewAction.Deleted || objMessage.ESourceViewAction == ESourceViewAction.Updated)
                Init();
        }

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        public void OnStatusUpdate(object sender, EventArgs e)
        {
            var strStatus = (string) sender;
            LabelStatus = strStatus;
        }

        private void InitDataTable()
        {
            DataTableItems = new ObservableCollection<DataTableItem>()
            {
                    new DataTableItem {Name = LabelTableAddress, Value = LabelTableAddressValue, Duration = DurationAddress, LastUpdate = LastUpdateAddress},
                    new DataTableItem {Name = LabelTableLastprice, Value = LabelTableLastpriceValue, Duration = DurationLastprice, LastUpdate = LastUpdateLastprice},
                    new DataTableItem {Name = LabelTableArticle, Value = LabelTableArticleValue, Duration = DurationArticle, LastUpdate = LastUpdateArticle},
                    new DataTableItem {Name = LabelTableArticleprice, Value = LabelTableArticlepriceValue, Duration = DurationArticleprice, LastUpdate = LastUpdateArticleprice},
                    new DataTableItem {Name = LabelTableOustandingpayments, Value = LabelTableOustandingpaymentsValue, Duration = DurationOutstandingpayments, LastUpdate = LastUpdateOutstandingpayments},
            };
        }

        private void ShowTotalDuration()
        {
            LabelStatus = string.Format("Total {0} {1}", TransferInfo.Instance.TotalDuration.ToString("F").Replace(",", "."), Language.seconds);
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            ControlIsEnabled = true;
            ListSources = Source.Instance.GetSources();

            LabelTitle = Language.Dataflow;

            LabelRenewAll = Language.RenewAll;
            LabelRenewUpdate = Language.Update;

            LabelName = Language.TableItemName;
            LabelValue = Language.TableItemCount;
            LabelDuration = Language.TableImportDuration;
            LabelLastUpdate = Language.Update;

            LabelTableAddress = "KONTEN";
            LabelTableArticle = "WAREN";
            LabelTableArticleprice = "A.PREISE";
            LabelTableLastprice = "L.PREISE";
            LabelTableOustandingpayments = "OP";

            InitDataTable();
            ShowTotalDuration();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties for section Listpicker / Buttons

        public double DurationAddress
        {
            get => TransferInfo.Instance.Find(typeof(Adress).Name).Duration;
            set
            {
                TransferInfo.Instance.Update(typeof(Adress).Name, value);
                DataTableItems[0].Duration = value;
                DataTableItems[0].Value = DataService.Count<Adress>();
                LastUpdateAddress = DateTime.Now;
                ShowTotalDuration();
                RaisePropertyChanged(() => DurationAddress);
                RaisePropertyChanged(() => LabelTableAddressValue);
            }
        }

        public DateTime LastUpdateAddress
        {
            get => TransferInfo.Instance.Find(typeof(Adress).Name).LastUpdate;
            set
            {
                TransferInfo.Instance.Update(typeof(Adress).Name, value);
                DataTableItems[0].LastUpdate = value;
                RaisePropertyChanged(() => LastUpdateAddress);
            }
        }

        public double DurationLastprice
        {
            get => TransferInfo.Instance.Find(typeof(Lastprice).Name).Duration;
            set
            {
                TransferInfo.Instance.Update(typeof(Lastprice).Name, value);
                DataTableItems[1].Duration = value;
                DataTableItems[1].Value = DataService.Count<Lastprice>();
                LastUpdateLastprice = DateTime.Now;
                ShowTotalDuration();
                RaisePropertyChanged(() => DurationLastprice);
                RaisePropertyChanged(() => LabelTableLastpriceValue);
            }
        }

        public DateTime LastUpdateLastprice
        {
            get => TransferInfo.Instance.Find(typeof(Lastprice).Name).LastUpdate;
            set
            {
                TransferInfo.Instance.Update(typeof(Lastprice).Name, value);
                DataTableItems[1].LastUpdate = value;
                RaisePropertyChanged(() => LastUpdateLastprice);
            }
        }

        public double DurationArticle
        {
            get => TransferInfo.Instance.Find(typeof(Article).Name).Duration;
            set
            {
                TransferInfo.Instance.Update(typeof(Article).Name, value);
                DataTableItems[2].Duration = value;
                DataTableItems[2].Value = DataService.Count<Article>();
                LastUpdateArticle = DateTime.Now;
                ShowTotalDuration();
                RaisePropertyChanged(() => DurationArticle);
                RaisePropertyChanged(() => LabelTableArticleValue);
            }
        }

        public DateTime LastUpdateArticle
        {
            get => TransferInfo.Instance.Find(typeof(Article).Name).LastUpdate;
            set
            {
                TransferInfo.Instance.Update(typeof(Article).Name, value);
                DataTableItems[2].LastUpdate = value;
                RaisePropertyChanged(() => LastUpdateArticle);
            }
        }

        public double DurationArticleprice
        {
            get => TransferInfo.Instance.Find(typeof(Articleprice).Name).Duration;
            set
            {
                TransferInfo.Instance.Update(typeof(Articleprice).Name, value);
                DataTableItems[3].Duration = value;
                DataTableItems[3].Value = DataService.Count<Articleprice>();
                LastUpdateArticleprice = DateTime.Now;
                ShowTotalDuration();
                RaisePropertyChanged(() => DurationArticleprice);
                RaisePropertyChanged(() => LabelTableArticlepriceValue);
            }
        }

        public DateTime LastUpdateArticleprice
        {
            get => TransferInfo.Instance.Find(typeof(Articleprice).Name).LastUpdate;
            set
            {
                TransferInfo.Instance.Update(typeof(Articleprice).Name, value);
                DataTableItems[3].LastUpdate = value;
                RaisePropertyChanged(() => LastUpdateArticleprice);
            }
        }

        public double DurationOutstandingpayments
        {
            get => TransferInfo.Instance.Find(typeof (OpenPayment).Name).Duration;
            set
            {
                TransferInfo.Instance.Update(typeof(OpenPayment).Name, value);
                DataTableItems[4].Duration = value;
                DataTableItems[4].Value = DataService.Count<OpenPayment>();
                LastUpdateOutstandingpayments = DateTime.Now;
                ShowTotalDuration();
                RaisePropertyChanged(() => DurationOutstandingpayments);
                RaisePropertyChanged(() => LabelTableOustandingpaymentsValue);
            }
        }

        public DateTime LastUpdateOutstandingpayments
        {
            get => TransferInfo.Instance.Find(typeof (OpenPayment).Name).LastUpdate;
            set
            {
                TransferInfo.Instance.Update(typeof(OpenPayment).Name, value);
                DataTableItems[4].LastUpdate = value;
                RaisePropertyChanged(() => LastUpdateOutstandingpayments);
            }
        }

        private ObservableCollection<Source> _listSources;
        public ObservableCollection<Source> ListSources
        {
            get => _listSources;
            set { _listSources = value; RaisePropertyChanged(() => ListSources); }
        }

        private string _labelName;
        public string LabelName { get => _labelName;
            set { _labelName = value; RaisePropertyChanged(() => LabelName); } }

        private string _labelValue;
        public string LabelValue { get => _labelValue;
            set { _labelValue = value; RaisePropertyChanged(() => LabelValue); } }

        private string _labelDuration;
        public string LabelDuration { get => _labelDuration;
            set { _labelDuration = value; RaisePropertyChanged(() => LabelDuration); } }

        private string _labelLastUpdate;
        public string LabelLastUpdate { get => _labelLastUpdate;
            set { _labelLastUpdate = value; RaisePropertyChanged(() => LabelLastUpdate); } }
       
        private string _labelStatus;
        public string LabelStatus
        {
            get => _labelStatus;
            set { _labelStatus = value; RaisePropertyChanged(() => LabelStatus); }
        }

        private string _labelRenewAll = "";
        public string LabelRenewAll
        {
            get => _labelRenewAll;
            set { _labelRenewAll = value; RaisePropertyChanged(() => LabelRenewAll); }
        }

        private string _labelRenewUpdate = "";
        public string LabelRenewUpdate
        {
            get => _labelRenewUpdate;
            set { _labelRenewUpdate = value; RaisePropertyChanged(() => LabelRenewUpdate); }
        }

        private string _labelRenewAddressData = "";
        public string LabelRenewAddressData
        {
            get => _labelRenewAddressData;
            set { _labelRenewAddressData = value; RaisePropertyChanged(() => LabelRenewAddressData); }
        }

        private string _labelRenewArticleData;
        public string LabelRenewArticleData
        {
            get => _labelRenewArticleData;
            set { _labelRenewArticleData = value; RaisePropertyChanged(() => LabelRenewArticleData); }
        }

        private string _labelRenewOutstandingpayments;
        public string LabelRenewOutstandingpayments
        {
            get => _labelRenewOutstandingpayments;
            set { _labelRenewOutstandingpayments = value; RaisePropertyChanged(() => LabelRenewOutstandingpayments); }
        }

        private string _labelRenewAccountData = "";
        public string LabelRenewAccountData
        {
            get => _labelRenewAccountData;
            set { _labelRenewAccountData = value; RaisePropertyChanged(() => LabelRenewAccountData); }
        }

        #endregion

        #region Public properties for showing content counts of tables

        private string _labelTableAddress;
        public string LabelTableAddress
        {
            get => _labelTableAddress;
            set { _labelTableAddress = value; RaisePropertyChanged(() => LabelTableAddress); }
        }
        public int LabelTableAddressValue => DataService.Count<Adress>();

        private string _labelTableArticle;
        public string LabelTableArticle
        {
            get => _labelTableArticle;
            set { _labelTableArticle = value; RaisePropertyChanged(() => LabelTableArticle); }
        }
        public int LabelTableArticleValue => DataService.Count<Article>();

        private string _labelTableArticleprice;
        public string LabelTableArticleprice
        {
            get => _labelTableArticleprice;
            set { _labelTableArticleprice = value; RaisePropertyChanged(() => LabelTableArticleprice); }
        }
        public int LabelTableArticlepriceValue => DataService.Count<Articleprice>();

        private string _labelTableOustandingpayments;
        public string LabelTableOustandingpayments
        {
            get => _labelTableOustandingpayments;
            set { _labelTableOustandingpayments = value; RaisePropertyChanged(() => LabelTableOustandingpayments); }
        }
        public int LabelTableOustandingpaymentsValue => DataService.Count<OpenPayment>();

        private string _labelTableLastprice;
        public string LabelTableLastprice
        {
            get => _labelTableLastprice;
            set { _labelTableLastprice = value; RaisePropertyChanged(() => LabelTableLastprice); }
        }
        public int LabelTableLastpriceValue => DataService.Count<Lastprice>();

        private ObservableCollection<DataTableItem> _dataTableItems;
        public ObservableCollection<DataTableItem> DataTableItems
        {
            get => _dataTableItems;
            set { _dataTableItems = value; RaisePropertyChanged(() => DataTableItems); }
        }

        public class DataTableItem : MvxNotifyPropertyChanged
        {
            private string _name;
            public string Name { get => _name;
                set { _name = value; RaisePropertyChanged(() => Name); } }

            private int _value;
            public int Value { get => _value;
                set { _value = value; RaisePropertyChanged(() => Value); } }

            private double _duration;
            public double Duration { get => _duration;
                set { _duration = value; RaisePropertyChanged(() => Duration); } }

            private DateTime _lastUpdate;
            public DateTime LastUpdate { get => _lastUpdate;
                set { _lastUpdate = value; RaisePropertyChanged(() => LastUpdate); } }
        }

        public enum EState
        {
            Downloading,
            Deserializing,
            Converting,
            Importing,
            Ready,
            Failed
        }

        #endregion

        #region Public properties for controls

        private bool _controlIsEnabled;
        public bool ControlIsEnabled
        {
            get => _controlIsEnabled;
            set
            {
                _controlIsEnabled = value;

                if(_controlIsEnabled)
                    DoHideWorkingCommand();
                else
                    DoShowWorkingCommand();

                RaisePropertyChanged(() => ControlIsEnabled);
            }
        }

        private bool _listPickerSourcesIsEnabled;
        public bool ListPickerSourcesIsEnabled
        {
            get => _listPickerSourcesIsEnabled;
            set { _listPickerSourcesIsEnabled = value; RaisePropertyChanged(() => ListPickerSourcesIsEnabled); }
        }

        private bool _buttonRenewAllIsEnabled;
        public bool ButtonRenewAllIsEnabled
        {
            get => _buttonRenewAllIsEnabled;
            set { _buttonRenewAllIsEnabled = value; RaisePropertyChanged(() => ButtonRenewAllIsEnabled); }
        }

        #endregion

        #region Public commands

        private MvxCommand<string> _renewAllCommand;
        public ICommand RenewAllCommand
        {
            get { return _renewAllCommand = _renewAllCommand ?? new MvxCommand<string>(DoRenewAllCommand); }
        }
        private void DoRenewAllCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        private MvxCommand<string> _renewUpdateCommand;
        public ICommand RenewUpdateCommand
        {
            get { return _renewUpdateCommand = _renewUpdateCommand ?? new MvxCommand<string>(DoRenewUpdateCommand); }
        }
        private void DoRenewUpdateCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        private MvxCommand<string> _renewAddressDataCommand;
        public ICommand RenewAddressDataCommand
        {
            get { return _renewAddressDataCommand = _renewAddressDataCommand ?? new MvxCommand<string>(DoRenewAddressDataCommand); }
        }
        private void DoRenewAddressDataCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        private MvxCommand<string> _renewArticleDataCommand;
        public ICommand RenewArticleDataCommand
        {
            get { return _renewArticleDataCommand = _renewArticleDataCommand ?? new MvxCommand<string>(DoRenewArticleDataCommand); }
        }
        private void DoRenewArticleDataCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        private MvxCommand<string> _renewOutstandingpaymentsCommand;
        public ICommand RenewOutstandingpaymentsCommand { get { _renewOutstandingpaymentsCommand = _renewOutstandingpaymentsCommand ?? new MvxCommand<string>(DoRenewOutstandingpaymentsCommand); return _renewOutstandingpaymentsCommand; } }
        private void DoRenewOutstandingpaymentsCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        private MvxCommand<string> _renewAccountDataCommand;
        public ICommand RenewAccountDataCommand { get { _renewAccountDataCommand = _renewAccountDataCommand ?? new MvxCommand<string>(DoRenewAccountDataCommand); return _renewAccountDataCommand; } }
        private void DoRenewAccountDataCommand(string strSourceName)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
            }
        }

        #endregion
    }
}
