using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public ChangePasswordViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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

            LabelUsername = Language.Username;
            LabelOldPassword = Language.OldPassword;
            LabelNewPassword1 = Language.NewPassword1;
            LabelNewPassword2 = Language.NewPassword2;
            LabelCancel = Language.Cancel;

            ControlIsEnabled = true;
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
        
	    private string _sourceName;
        public string SourceName { get => _sourceName;
            set { _sourceName = value; RaisePropertyChanged(() => SourceName); } }

        private string _labelUsername;
        public string LabelUsername { get => _labelUsername;
            set { _labelUsername = value; RaisePropertyChanged(() => LabelUsername); } }

	    private string _username;
        public string Username { get => _username;
            set { _username = value; RaisePropertyChanged(() => Username); } }

	    private string _labelOldPassword;
        public string LabelOldPassword { get => _labelOldPassword;
            set { _labelOldPassword = value; RaisePropertyChanged(() => LabelOldPassword); } }

	    private string _oldPassword;
        public string OldPassword { get => _oldPassword;
            set { _oldPassword = value; RaisePropertyChanged(() => OldPassword); } }

	    private string _labelNewPassword1;
	    public string LabelNewPassword1 { get => _labelNewPassword1;
            set { _labelNewPassword1 = value; RaisePropertyChanged(() => LabelNewPassword1); } }

	    private string _newPassword1;
        public string NewPassword1 { get => _newPassword1;
            set { _newPassword1 = value; RaisePropertyChanged(() => NewPassword1); } }

        private string _labelNewPassword2;
        public string LabelNewPassword2 { get => _labelNewPassword2;
            set { _labelNewPassword2 = value; RaisePropertyChanged(() => LabelNewPassword2); } }

        private string _newPassword2;
        public string NewPassword2 { get => _newPassword2;
            set { _newPassword2 = value; RaisePropertyChanged(() => NewPassword2); } }

	    private string _labelCancel;
	    public string LabelCancel { get => _labelCancel;
            set { _labelCancel = value; RaisePropertyChanged(() => LabelCancel); } }

        #endregion

    }
}
