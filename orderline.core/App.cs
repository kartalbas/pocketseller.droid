using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;

namespace pocketseller.core
{
    public class App : MvxApplication
    {
        public static string BackendToken { get; set; }
            
        public override void Initialize()
        {
            //Register all services
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterSingleton(() => UserDialogs.Instance);

            InitSettingDatabase();
            InitDefaultPocketsellerDatabase();

            RegisterAppStart<LoginViewModel>();
        }

        private void InitSettingDatabase()
        {
            var objDataService = ((CDataService)Mvx.IoCProvider.Resolve<IDataService>());
            objDataService.CreateSettingDb();

            var objSettingService = ((CSettingService)Mvx.IoCProvider.Resolve<ISettingService>());
            objSettingService.InitSettings();
        }

        private void InitDefaultPocketsellerDatabase()
        {
            var objDataService = ((CDataService)Mvx.IoCProvider.Resolve<IDataService>());
            var objCurrentSource = Source.Instance.GetCurrentSource();

            string strDatabasename = objCurrentSource == null ? "dummy.db" : objCurrentSource.DbName;

            objDataService.CreatePocketsellerDb(strDatabasename);
        }
    }
}