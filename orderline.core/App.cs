using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using System.Collections.Generic;

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
            InitSources();
            InitDefaultPocketsellerDatabase();

            RegisterAppStart<LoginViewModel>();
        }

        private void InitSources()
        {
            var sources = new List<Source>
            {
                new Source { Id = 1, Name = "DOR", Host = "activator.yilmazfeinkost.de:5001", DbName = "DOR.db" },
                new Source { Id = 2, Name = "DORT", Host = "activator.yilmazfeinkost.de:5031", DbName = "DORT.db" },
                new Source { Id = 3, Name = "KOL", Host = "activator.yilmazfeinkost.de:5021", DbName = "KOL.db" },
                new Source { Id = 4, Name = "KOLT", Host = "activator.yilmazfeinkost.de:5031", DbName = "KOLT.db" },
                new Source { Id = 5, Name = "NEU", Host = "activator.yilmazfeinkost.de:5041", DbName = "NEU.db" },
                new Source { Id = 6, Name = "NEUT", Host = "activator.yilmazfeinkost.de:5051", DbName = "NEUT.db" },
                new Source { Id = 6, Name = "DEV", Host = "activator.yilmazfeinkost.de:6001", DbName = "DEV.db" }
            };

            Source.Instance.DeleteAll();
            Source.Instance.Save(sources);
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
            var objDataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();
            var objCurrentSource = Source.Instance.GetCurrentSource();

            string strDatabasename = objCurrentSource == null ? "dummy.db" : objCurrentSource.DbName;
            objDataService.CreatePocketsellerDb(strDatabasename);
        }
    }
}