using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCross;
using pocketseller.core.Services.Interfaces;
using SQLite;

// ReSharper disable once CheckNamespace
namespace pocketseller.core.Services
{
    public partial class EMails
    {
        public static CDataService DataService { get; set; }
        public static CSettingService SettingService { get; set; }

        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }
        public string Mail { get; set; }
        public DateTime TimeStamp { get; set; }

        public EMails()
        {
            TimeStamp = DateTime.Now;

            if (DataService == null)
                DataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();

            if (SettingService == null)
                SettingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
        }

        private static EMails _objInstance;
        public static EMails Instance => _objInstance ?? (_objInstance = new EMails());

        public void Kill()
        {
            _objInstance = null;
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(Mail))
                return false;

            if (Find(this) != null)
                DataService.SettingsConnection.Update(this);
            else
                DataService.SettingsConnection.Insert(this);

            return true;
        }

        public void Save(IList<EMails> mails)
        {
            DeleteAll();

            foreach (var mail in mails.OrderBy(a => a.Mail))
            {
                mail.Save();
            }
        }

        public void Delete()
        {
            DataService.SettingsConnection.Delete(this);
        }

        public void DeleteAll()
        {
            DataService.SettingsConnection.DeleteAll<EMails>();            
        }

        public EMails GetCurrentEMail()
        {
            var mail = SettingService.Get<string>(ESettingType.OpManager);
            return (DataService.SettingsConnection.Table<EMails>().Where(s => s.Mail == mail)).FirstOrDefault();
        }

        public EMails Find(EMails mail)
        {
            return (DataService.SettingsConnection.Table<EMails>().Where(s => s.Id == mail.Id)).FirstOrDefault();
        }

        public ObservableCollection<string> GetMails()
        {
            var mails = DataService.SettingsConnection.Table<EMails>().OrderBy(o => o.Mail).ToList();
            return new ObservableCollection<string>(mails.SelectMany(m => new[] { m.Mail }));
        }

        public ObservableCollection<EMails> GetEMails()
        {
            var mails = DataService.SettingsConnection.Table<EMails>().OrderBy(o => o.Mail).ToList();
            return new ObservableCollection<EMails>(mails);
        }

        public override string ToString()
        {
            return Mail;
        }
    }
}
