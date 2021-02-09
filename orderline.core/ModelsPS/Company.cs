using System;
using MvvmCross;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

// ReSharper disable once CheckNamespace
namespace pocketseller.core.Models
{
    public partial class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Street { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Bankname { get; set; }

        public string BankIban { get; set; }

        public string BankSwiftBic { get; set; }

        public string UstId { get; set; }

        public string Text { get; set; }

        public string Mail { get; set; }

        public string Subject { get; set; }

        public string OrderFile { get; set; }

        public string Body { get; set; }

        public static CDataService DataService { get; set; }
        public static CSettingService SettingService { get; set; }

        public DateTime TimeStamp { get; set; }

        public Company()
        {
            TimeStamp = DateTime.Now;

            if (DataService == null)
                DataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();

            if (SettingService == null)
                SettingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
        }

        private static Company _objInstance;
        public static Company Instance => _objInstance ?? (_objInstance = new Company());

        public void Kill()
        {
            _objInstance = null;
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(Mail))
                return false;

            if (Get() != null)
                DataService.PocketsellerConnection.Update(this);
            else
                DataService.PocketsellerConnection.Insert(this);

            return true;
        }

        public Company Get()
        {
            var result = DataService.PocketsellerConnection.Table<Company>().FirstOrDefault();
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
