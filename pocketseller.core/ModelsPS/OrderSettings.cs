using System.Linq;

namespace pocketseller.core.Models
{
    public class OrderSettings : BaseModel
    {
        private static OrderSettings _objInstance;

        public OrderSettings()
        {
            AutoPrice = 0;
            DefaultDocNr = 0;
            CurrentDocNr = 0;
            CurrentQuoNr = 0;
            VKMustHigherPercent = 0;
            VKMustLowerPercent = 0;
            MaxDocumentdetails = 0;
            CheckStock = true;
            CheckVKMustHigher = true;
            _pictureUrl = "https://pic.yilmazfeinkost.de";
        }

        private int _autoPrice;
        public int AutoPrice{ get => _autoPrice;
            set  {  _autoPrice = value; Update(); RaisePropertyChanged(() => AutoPrice); } }

        private int _defaultDocNr;
        public int DefaultDocNr { get => _defaultDocNr;
            set { _defaultDocNr = value; Update(); RaisePropertyChanged(() => DefaultDocNr); } }

        private int _currentDocNr;
        public int CurrentDocNr { get => _currentDocNr;
            set { _currentDocNr = value; Update(); RaisePropertyChanged(() => CurrentDocNr); } }

        private int _currentQuoNr;
        public int CurrentQuoNr { get => _currentQuoNr;
            set { _currentQuoNr = value; Update(); RaisePropertyChanged(() => CurrentQuoNr); } }

        private int _vkMustHigherPercent;
        public int VKMustHigherPercent { get => _vkMustHigherPercent;
            set { _vkMustHigherPercent = value; Update(); RaisePropertyChanged(() => VKMustHigherPercent); } }

        private int _vkMustLowerPercent;
        public int VKMustLowerPercent { get => _vkMustLowerPercent;
            set { _vkMustLowerPercent = value; Update(); RaisePropertyChanged(() => VKMustLowerPercent); } }

        private bool _checkStock;
        public bool CheckStock { get => _checkStock;
            set { _checkStock = value; Update(); RaisePropertyChanged(() => CheckStock); } }

        private bool _checkVKMustHigher;
        public bool CheckVKMustHigher { get => _checkVKMustHigher;
            set { _checkVKMustHigher = value; Update(); RaisePropertyChanged(() => CheckVKMustHigher); } }

        private int _maxDocumentdetails;
        public int MaxDocumentdetails { get => _maxDocumentdetails;
            set { _maxDocumentdetails = value; Update(); RaisePropertyChanged(() => MaxDocumentdetails); } }

        private string _pictureUrl;
        public string PictureUrl { get => _pictureUrl;
            set { _pictureUrl = value; Update(); RaisePropertyChanged(() => PictureUrl); } }

        private void Update()
        {
            DataService.PocketsellerConnection.Update(this);
        }

        public void Kill()
        {
            _objInstance = null;
        }

        public static OrderSettings Instance
        {
            get
            {
                if (_objInstance == null)
                    _objInstance = Get();

                return _objInstance;
            }
        }

        private static OrderSettings Get()
        {
            if (Table<OrderSettings>().Any())
            {
                return DataService.PocketsellerConnection.Table<OrderSettings>().FirstOrDefault();
            }

            DataService.PocketsellerConnection.Insert(new OrderSettings
            {
                AutoPrice = 0,
                DefaultDocNr = 1000,
                CurrentDocNr = 1000,
                VKMustHigherPercent = 0,
                VKMustLowerPercent = 200,
                MaxDocumentdetails = 1000,
                CheckStock = true,
                CheckVKMustHigher = true,
                PictureUrl = string.Empty
            });

            return Table<OrderSettings>().FirstOrDefault();
        }
    }
}
