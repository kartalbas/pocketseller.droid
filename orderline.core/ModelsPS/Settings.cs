namespace pocketseller.core.Models
{
    public class Settings : BaseModel
    {
        private static Settings _objInstance;

        public Settings()
        {
            Key = string.Empty;
            Value = string.Empty;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public static Settings Instance
        {
            get
            {
                if(_objInstance == null)
                    _objInstance = new Settings();

                return _objInstance;
            }
        }

        public void Kill()
        {
            _objInstance = null;
        }

        public Settings FindSetting(string strKey)
        {
            return (DataService.SettingsConnection.Table<Settings>().Where(s => s.Key == strKey)).FirstOrDefault();
        }

        public void Insert(Settings objSetting)
        {
            DataService.SettingsConnection.Insert(objSetting);
        }

        public void Update(Settings objSetting)
        {
            DataService.SettingsConnection.Update(objSetting);
        }
    }
}
