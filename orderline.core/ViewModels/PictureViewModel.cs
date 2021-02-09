using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class PictureViewModel : BaseViewModel
	{
		#region Private properties
		#endregion

		#region Constructors

        public PictureViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
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
            LabelTitle = Language.Picture;
            LabelMenuTitle = string.Empty;
            LabelCancel = Language.Cancel;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        private string _labelCancel;
        public string LabelCancel { get => _labelCancel;
            set { _labelCancel = value; RaisePropertyChanged(() => LabelCancel); } }

	    public string ImageUrl => DocumentService.Documentdetail.Article.ImageUrl(-1);
        public string ImageUrlDefault => DocumentService.Documentdetail.Article.ImageUrl(-1, "default");
        public string ImageUrlError => DocumentService.Documentdetail.Article.ImageUrl(-1, "error");

        #endregion

        #region ICommand implementations
        #endregion
    }
}
