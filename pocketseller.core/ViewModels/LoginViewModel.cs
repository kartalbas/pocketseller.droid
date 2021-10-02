using System;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using pocketseller.core.Tools;
using Acr.UserDialogs;

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
            LabelBranch = Language.Branch;
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
        
        private string _labelBranch;
	    public string LabelBranch
        {
            get => _labelBranch;
            set { _labelBranch = value; RaisePropertyChanged(() => LabelBranch); }
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
            set 
            {
                _username = value;
                RaisePropertyChanged(() => Username); 
            }
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

        private string _branch;
        public string Branch
        {
            get => _branch;
            set { _branch = value; RaisePropertyChanged(() => Branch); }
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

        private string DomainMapper(Match match)
        {
            string domainName = new IdnMapping().GetAscii(match.Groups[2].Value);
            return match.Groups[1].Value + domainName;
        }

        public async Task<Tuple<string, string, string>> GetMobile()
        {
            try
            {
                var dialag = Mvx.IoCProvider.Resolve<IUserDialogs>();
                var rest = Mvx.IoCProvider.Resolve<IRestService>();

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Branch))
                {
                    dialag.Toast($"Check {Language.Branch}/{Language.Mail}/{Language.Password}", TimeSpan.FromSeconds(3));
                    return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
                }

                var source = Source.Instance.FindByName(Branch);
                if(source == null)
                {
                    dialag.Toast($"Check {Language.Branch}", TimeSpan.FromSeconds(3));
                    return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
                }

                var valid = RegexUtilities.IsValidEmail(Username);
                if (!valid)
                {
                    dialag.Toast($"Check {Language.Mail}", TimeSpan.FromSeconds(3));
                    return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
                }

                var result = await rest.GetMobileNumber(Username, Password, source.Name);

                if(string.IsNullOrEmpty(result))
                {
                    dialag.Toast(Language.CouldNotBeActivated, TimeSpan.FromSeconds(3));
                    return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
                }

                return new Tuple<string, string, string>(source.Name, Username, result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
