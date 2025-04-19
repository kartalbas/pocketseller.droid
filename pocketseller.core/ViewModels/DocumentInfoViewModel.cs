using MvvmCross.Plugin.Messenger;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentInfoViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public DocumentInfoViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
        }

        #endregion

        #region Private methods

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.TabInfo;
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        public string TextInfo { get => DocumentService.Document.Info;
            set { DocumentService.Document.Info = value; RaisePropertyChanged(() => TextInfo); } }

        #endregion

        #region ICommand implementations
        #endregion
    }
}
