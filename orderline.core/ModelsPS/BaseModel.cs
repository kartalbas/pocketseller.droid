using System;
using MvvmCross;
using MvvmCross.ViewModels;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using SQLite;

namespace pocketseller.core.Models
{
    public class BaseModel : MvxNotifyPropertyChanged
    {
        protected static string LogTag { get; set; }
        protected static bool DisableLoading { get; set; }
        protected object Lock;

        public static CDataService DataService { get; set; }
        public static CSettingService SettingService { get; set; }

        public static CDocumentService DocumentService { get; set; }

        public BaseModel()
        {
            Lock = new object();
            LogTag = GetType().Name;
            TimeStamp = DateTime.Now;
            DisableLoading = false;

            if (DataService == null)
                DataService = (CDataService) Mvx.IoCProvider.Resolve<IDataService>();

            if(SettingService == null)
                SettingService = (CSettingService) Mvx.IoCProvider.Resolve<ISettingService>();            
        }

        protected static TableQuery<T> Table<T>() where T : new()
        {
            var table = DataService.PocketsellerConnection.Table<T>();
            return table;
        }

        private Guid _id;
        private DateTime _timeStamp;

        [PrimaryKey, Indexed]
        public Guid Id { get => _id; set { _id = value; RaisePropertyChanged(() => Id); }}
        public DateTime TimeStamp { get => _timeStamp; set { _timeStamp = value; RaisePropertyChanged(() => TimeStamp); } }
    }
}
