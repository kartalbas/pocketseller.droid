using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross;
using pocketseller.core.Services.Interfaces;
using SQLite;

// ReSharper disable once CheckNamespace
namespace pocketseller.core.Services
{
    public partial class Source
    {
        public static CDataService DataService { get; set; }
        public static CSettingService SettingService { get; set; }

        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }
        public string Application { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string Host { get; set; }
        public string DbName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime TimeStamp { get; set; }

        public Source()
        {
            TimeStamp = DateTime.Now;

            if (DataService == null)
                DataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();

            if (SettingService == null)
                SettingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
        }

        private static Source _objInstance;
        public static Source Instance => _objInstance ?? (_objInstance = new Source());

        public void Kill()
        {
            _objInstance = null;
        }

        public bool Save()
        {
            if (Name == string.Empty || DbName == string.Empty || Host == string.Empty)
                return false;

            if (Find(this) != null)
                DataService.SettingsConnection.Update(this);
            else
                DataService.SettingsConnection.Insert(this);

            return true;
        }

        public void Save(List<Source> cobjSources)
        {
            DeleteAll();

            foreach (var objSource in cobjSources.OrderBy(a => a.Name))
            {
                objSource.Save();
            }
        }

        public void Delete()
        {
            DataService.SettingsConnection.Delete(this);
        }

        public void DeleteAll()
        {
            DataService.SettingsConnection.DeleteAll<Source>();
        }

        public string GetCurrentName()
        {
            Source objSource = GetCurrentSource();

            if (objSource != null && objSource.Name != null)
                return objSource.Name;
            return string.Empty;
        }

        public string GetCurrentDestinationMail()
        {
            Source objSource = GetCurrentSource();

            if (objSource != null && objSource.DbName != null)
                return objSource.DbName;
            return string.Empty;
        }

        public string GetCurrentSourceFolder()
        {
            Source objSource = GetCurrentSource();

            if (objSource != null && objSource.Host != null)
                return objSource.Host;
            return string.Empty;
        }

        public Source GetCurrentSource()
        {
            var strSourceName = SettingService.Get<string>(ESettingType.DataSourceName);
            return DataService.SettingsConnection.Table<Source>().Where(s => s.Name == strSourceName).FirstOrDefault();
        }

        public Source Find(Source objSource)
        {
            return DataService.SettingsConnection.Table<Source>().Where(s => s.Id == objSource.Id).FirstOrDefault();
        }

        public Source FindBy(int id)
        {
            return DataService.SettingsConnection.Table<Source>().Where(s => s.Id == id).FirstOrDefault();
        }

        public Source GetSourceByName(string strSourceName)
        {
            return (DataService.SettingsConnection.Table<Source>().Where(s => s.Name == strSourceName)).FirstOrDefault();
        }

        public ObservableCollection<Source> GetSources()
        {
            var cobjSources = DataService.SettingsConnection.Table<Source>().OrderBy(o => o.Name).ToList();
            return new ObservableCollection<Source>(cobjSources);
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetApiUrl(string strHost)
        {
            return string.Format(@"{0}://{1}/{2}", SettingService.Get<string>(ESettingType.RestProtocol), strHost, SettingService.Get<string>(ESettingType.RestDataApi));
        }

        public string GetLoginUrl(string strHost)
        {
            return string.Format(@"{0}://{1}/{2}", SettingService.Get<string>(ESettingType.RestProtocol), strHost, SettingService.Get<string>(ESettingType.RestLoginApi));
        }

        public string GetMailUrl(string strHost)
        {
            return string.Format(@"{0}://{1}/{2}", SettingService.Get<string>(ESettingType.RestProtocol), strHost, "api/v1/login");
        }

        public string GetResourceUrl(string strHost)
        {
            return string.Format(@"{0}://{1}/{2}", SettingService.Get<string>(ESettingType.RestProtocol), strHost, SettingService.Get<string>(ESettingType.RestDataResource));
        }
    }
}
