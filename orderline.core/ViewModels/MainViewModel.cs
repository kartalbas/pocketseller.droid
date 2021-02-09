using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;

namespace pocketseller.core.ViewModels
{

    public class MainMenu
    {
        public string ImagePath { get; set; }
        public string Name { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;

            DataInterfaceViewModel = CMvvmCrossTools.LoadViewModel<DataInterfaceViewModel>();
            SettingViewModel = CMvvmCrossTools.LoadViewModel<SettingViewModel>();
            QuotationsViewModel = CMvvmCrossTools.LoadViewModel<QuotationsViewModel>();
            DocumentViewModel = CMvvmCrossTools.LoadViewModel<DocumentViewModel>();
            DocumentsViewModel = CMvvmCrossTools.LoadViewModel<DocumentsViewModel>();
            StockViewModel = CMvvmCrossTools.LoadViewModel<StockViewModel>();
            FacturaViewModel = CMvvmCrossTools.LoadViewModel<FacturaViewModel>();

            SubscriptionToken1 = Messenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
            SubscriptionToken2 = objMessenger.SubscribeOnMainThread<SourceViewMessage>(OnSourceChanged);

            LanguageService.ChangeLanguageThroughSettings();
        }

        #region Private methods

        private void OnSourceChanged(SourceViewMessage objMessage)
        {
            if (objMessage.ESourceViewAction == ESourceViewAction.Updated)
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
            LabelNewDocument = Language.NewOrder;
            LabelDocuments = Language.Orders;
            LabelQuotations = Language.Quotations;
            LabelStock = Language.Stock;
            LabelFactura = Language.PrintFactura;
            LabelSettings = Language.Settings;
            LabelDataflow = Language.Dataflow;
            LabelLogout = Language.Logout;

            ListMenu = new ObservableCollection<MainMenu>
            {
               new MainMenu { ImagePath = "ic_action_gamepad_dark.png", Name = LabelDocuments },
               new MainMenu { ImagePath = "ic_action_unread_dark.png", Name = LabelQuotations },
               new MainMenu { ImagePath = "ic_action_cloud_dark.png", Name = LabelStock },
               new MainMenu { ImagePath = "ic_action_cloud_dark.png", Name = LabelFactura },
               new MainMenu { ImagePath = "ic_action_import_export_dark.png", Name = LabelDataflow },
               new MainMenu { ImagePath = "ic_action_settings_dark.png", Name = LabelSettings },
               new MainMenu { ImagePath = "ic_action_next_item_dark.png", Name = LabelLogout }
            };
        }

        public override void Init(object objParam) { }

        #endregion

        #region Private methods

        #endregion

        #region Public properties

        public DocumentsViewModel DocumentsViewModel { get; set; }
        public DataInterfaceViewModel DataInterfaceViewModel { get; set; }
        public SettingViewModel SettingViewModel { get; set; }
        public QuotationsViewModel QuotationsViewModel { get; set; }
        public DocumentViewModel DocumentViewModel { get; set; }
        public StockViewModel StockViewModel { get; set; }
        public FacturaViewModel FacturaViewModel { get; set; }

        private string _labelNewDocument;
        public string LabelNewDocument
        {
            get => _labelNewDocument;
            set { _labelNewDocument = value; RaisePropertyChanged(() => LabelNewDocument); }
        }

        private ObservableCollection<MainMenu> _listMenu; 
        public ObservableCollection<MainMenu> ListMenu
        {
            get => _listMenu;
            set { _listMenu = value; RaisePropertyChanged(() => ListMenu); }
        }

        private string _labelDocuments;
        public string LabelDocuments
        {
            get => _labelDocuments;
            set { _labelDocuments = value; RaisePropertyChanged(() => LabelDocuments); }
        }

        private string _labelQuotations;
        public string LabelQuotations
        {
            get => _labelQuotations;
            set { _labelQuotations = value; RaisePropertyChanged(() => LabelQuotations); }
        }

        private string _labelStock;
        public string LabelStock { get => _labelStock;
            set { _labelStock = value; RaisePropertyChanged(() => LabelStock); } }

        private string _labelFactura;
        public string LabelFactura
        {
            get => _labelFactura;
            set { _labelFactura = value; RaisePropertyChanged(() => LabelFactura); }
        }

        private string _labelLogout;
        public string LabelLogout { get => _labelLogout;
            set { _labelLogout = value; RaisePropertyChanged(() => LabelLogout); } }

        private string _labelSettings;
        public string LabelSettings
        {
            get => _labelSettings;
            set { _labelSettings = value; RaisePropertyChanged(() => LabelSettings); }
        }

        private string _labelDataflow;
        public string LabelDataflow
        {
            get => _labelDataflow;
            set { _labelDataflow = value; RaisePropertyChanged(() => LabelDataflow); }
        }
        
        private MvxCommand _showMainViewCommand;
        public ICommand ShowLoginViewCommand { get { _showMainViewCommand = _showMainViewCommand ?? new MvxCommand(DoShowMainViewCommand); return _showMainViewCommand; } }
        private void DoShowMainViewCommand()
        {
            LogoutCommand.Execute(null);
            NavigationService.Navigate<LoginViewModel>();
        }

        #endregion
    }
}
