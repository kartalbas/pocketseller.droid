using System.Globalization;
using System.Threading;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.Services
{
    public class CLanguageService : CBaseService, ILanguageService, IBaseService
    {
        private CSettingService SettingService { get; set; }

        public CLanguageService(IMvxMessenger objMessenger, ISettingService objSettingService)
            : base(objMessenger)
        {
            LogTag = GetType().Name;
            SettingService = (CSettingService)objSettingService;
            ChangeLanguageThroughSettings();            
        }

        public ELanguage ConvertLanguage(int iIndex)
        {
            return (ELanguage)iIndex;
        }

        public void ChangeLanguageThroughSettings()
        {
            var enmLanguage = SettingService.Get<ELanguage>(ESettingType.Language);
            ChangeLanguage(enmLanguage);
        }

        public void ChangeLanguage(ELanguage enmLanguage)
        {
            var newCulture = new CultureInfo(GetLanCode(enmLanguage));
            Language.Culture = newCulture;

            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            Messenger.Publish(new LanguageServiceMessage(this, true));
        }

        private static string GetLanCode(ELanguage enmLanguage)
        {
            return enmLanguage.ToString().Replace("_", "-");
        }
    }
}
