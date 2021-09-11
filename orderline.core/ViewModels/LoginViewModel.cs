using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class LoginViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public LoginViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.Login;
            LabelMenuTitle = string.Empty;

            LabelMenuTitle = string.Empty;
            LabelCode = Language.Code;
            LabelUsername = Language.Username;
            LabelPassword = Language.Password;
            LabelLogin = Language.Login;
            LabelActivate = Language.Activate;
            LabelExit = Language.Exit;
            LabelChangePassword = Language.ChangePassword;

            ControlIsEnabled = true;
            ActivationInitialized = false;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private bool _controlIsEnabled;
        public bool ControlIsEnabled
        {
            get => _controlIsEnabled;
            set
            {
                _controlIsEnabled = value;

                if (_controlIsEnabled)
                    DoHideWorkingCommand();
                else
                    DoShowWorkingCommand();

                RaisePropertyChanged(() => ControlIsEnabled);
            }
        }
        
        private string _labelCode;
	    public string LabelCode
        {
            get => _labelCode;
            set { _labelCode = value; RaisePropertyChanged(() => LabelCode); }
        }

        public bool ActivationInitialized { get; set; }

        private ObservableCollection<EMails> _listEMails;
        public ObservableCollection<EMails> ListEMails
        {
            get => _listEMails;
            set
            {
                _listEMails = value;
                EMails.Instance.Save(_listEMails.ToList());
                RaisePropertyChanged(() => ListEMails);
            }
        }
                
        private string _labelUsername;
        public string LabelUsername
        {
            get => _labelUsername;
            set { _labelUsername = value; RaisePropertyChanged(() => LabelUsername); }
        }

	    private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; RaisePropertyChanged(() => Username); }
        }

	    private string _labelPassword;
        public string LabelPassword
        {
            get => _labelPassword;
            set { _labelPassword = value; RaisePropertyChanged(() => LabelPassword); }
        }

	    private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; RaisePropertyChanged(() => Password); }
        }

        private string _code;
        public string Code
        {
            get => _code;
            set { _code = value; RaisePropertyChanged(() => Code); }
        }
        
        private string _labelLogin;
	    public string LabelLogin
        {
            get => _labelLogin;
            set { _labelLogin = value; RaisePropertyChanged(() => LabelLogin); }
        }

        private string _labelExit;
	    public string LabelExit
        {
            get => _labelExit;
            set { _labelExit = value; RaisePropertyChanged(() => LabelExit); }
        }

	    private string _labelActivate;
	    public string LabelActivate
        {
            get => _labelActivate;
            set { _labelActivate = value; RaisePropertyChanged(() => LabelActivate); }
        }

	    private string _labelChangePassword;
        public string LabelChangePassword
        {
            get => _labelChangePassword;
            set { _labelChangePassword = value; RaisePropertyChanged(() => LabelChangePassword); }
        }

	    public bool Login
	    {
	        get
	        {
                //TODO: implement login here

                //TODO: get emails
                //ListEMails = await client.GetMailsAsync();

                SettingService.Set(ESettingType.LoginTime, DateTime.Now);
                Mvx.IoCProvider.Resolve<IRestService>()?.GetToken().ContinueWith(t => App.BackendToken = t.Result);
                return true;
	        }
	    }

        private MvxCommand _showMainViewCommand;
        public ICommand ShowMainViewCommand { get { _showMainViewCommand = _showMainViewCommand ?? new MvxCommand(DoShowMainViewCommand); return _showMainViewCommand; } }
        private void DoShowMainViewCommand()
        {
            if (CheckLoginTimeOut)
            {
                NavigationService.Navigate<MainViewModel>();
            }
        }

        #endregion
    }
}
