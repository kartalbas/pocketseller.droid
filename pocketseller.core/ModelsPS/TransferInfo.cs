using System;
using System.Linq;

namespace pocketseller.core.Models
{
    public class TransferInfo : BaseModel
    {
        private static TransferInfo _objInstance;

        public TransferInfo()
        {
            Table = string.Empty;
            Duration = 0;
        }

        private string _table;
        public string Table { get => _table;
            set { _table = value; RaisePropertyChanged(() => Table); } }

        private double _duration;
        public double Duration { get => _duration;
            set { _duration = value; RaisePropertyChanged(() => Duration); } }

        private DateTime _lastUpdate;
        public DateTime LastUpdate { get => _lastUpdate;
            set { _lastUpdate = value; RaisePropertyChanged(() => LastUpdate); } }

        public static TransferInfo Instance
        {
            get
            {
                if (_objInstance == null)
                {
                    _objInstance = new TransferInfo();
                    Init();                    
                }

                return _objInstance;
            }
        }

        public void Kill()
        {
            _objInstance = null;
        }

        private static void Init()
        {
            DataService.InsertOrUpdate(new TransferInfo { Id = Guid.NewGuid(), Table = typeof(Adress).Name, Duration = 0, LastUpdate = default(DateTime) });
            DataService.InsertOrUpdate(new TransferInfo { Id = Guid.NewGuid(), Table = typeof(Article).Name, Duration = 0, LastUpdate = default(DateTime) });
            DataService.InsertOrUpdate(new TransferInfo { Id = Guid.NewGuid(), Table = typeof(Articleprice).Name, Duration = 0, LastUpdate = default(DateTime) });
            DataService.InsertOrUpdate(new TransferInfo { Id = Guid.NewGuid(), Table = typeof(Lastprice).Name, Duration = 0, LastUpdate = default(DateTime) });
            DataService.InsertOrUpdate(new TransferInfo { Id = Guid.NewGuid(), Table = typeof(OpenPayment).Name, Duration = 0, LastUpdate = default(DateTime) });
        }

        public TransferInfo Find(string strTableName)
        {
            var result = DataService.PocketsellerConnection.Table<TransferInfo>()
                .Where(s => s.Table == strTableName)
                .FirstOrDefault();

            return result;
        }

        public void Update(string strTableName, double dDuration)
        {
            var objCurrent = Find(strTableName);
            objCurrent.Duration = dDuration;
            DataService.PocketsellerConnection.Update(objCurrent);
        }

        public void Update(string strTableName, DateTime dtLastUpdate)
        {
            var objCurrent = Find(strTableName);
            objCurrent.LastUpdate = dtLastUpdate;
            DataService.PocketsellerConnection.Update(objCurrent);
        }

        public double TotalDuration
        {
            get
            {
                return DataService.PocketsellerConnection.Table<TransferInfo>()
                    .Sum(d => d.Duration);
            }
        }
    }
}
