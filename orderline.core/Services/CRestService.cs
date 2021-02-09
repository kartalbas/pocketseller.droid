using pocketseller.core.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using MvvmCross.Plugin.Messenger;
using orderline.core.ModelsPS;

namespace pocketseller.core.Services
{
    public class CRestService : CBaseService, IRestService, IBaseService
    {
        private string _deviceId;
        private string _id;
        private string _key;
        private string _activatorServerUri;
        private ISettingService _settingService;
        IBasicPlatformService _platformService;

        public CRestService(IMvxMessenger messenger, ISettingService settingService, IBasicPlatformService platformService) : base(messenger)
        {
            _settingService = settingService;
            _platformService = platformService;
            _settingService = settingService;
            _deviceId = "Pocketseller" + "_" + _platformService.GetDeviceIdentification();
            _id = "771e28e5-1da7-4af6-bc65-34fd74231d76";
            _key = "8c7920a1-a599-4981-af40-6448222aa4a3";

            _activatorServerUri = settingService.Get<string>(ESettingType.ActivatorUrl);
        }

        public async Task<FacturaData> GetFacturaDataAsync(int orderNumber)
        {
            try
            {
                var client = new HttpClient();
                var methodUrl = GetPocketsellerHost("GetFacturaData");
                var serverUri = new Uri($"{methodUrl}/{orderNumber}");
                Console.WriteLine(serverUri.ToString());
                var response = await client.GetAsync(serverUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FacturaData>(content);
                    result.OpenPayments = FacturaData.FormatOpenPayments(result.OpenPayments);
                    result.PaymentSheets = FacturaData.FormatPaymentSheet(result.PaymentSheets);
                    result.TotalOpenAmount = FacturaData.GetTotalOpenAmount(result.OpenPayments);
                    return result;
                }

                return new FacturaData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EMails>> GetMailsAsync()
        {
            try
            {
                var client = new HttpClient();
                var serverUri = new Uri($"{_activatorServerUri}/Activator/GetMails/{_deviceId}/{_id}/{_key}");
                var response = await client.GetAsync(serverUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ObservableCollection<EMails>>(content);
                    return result;
                }

                return new ObservableCollection<EMails>();
            }
            catch (Exception)
            {
                return new ObservableCollection<EMails>();
            }
        }

        public async Task<ObservableCollection<Source>> GetSourcesAsync()
        {
            try
            {
                var client = new HttpClient();
                var serverUri = new Uri($"{_activatorServerUri}/Activator/GetSources/{_deviceId}/{_id}/{_key}");
                var response = await client.GetAsync(serverUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ObservableCollection<Source>>(content);
                    return result;
                }

                return new ObservableCollection<Source>();
            }
            catch (Exception)
            {
                throw new Exception("Gerät ist nicht registriert");
            }
        }

        private string GetPocketsellerHost(string method)
        {
            var source = Source.Instance.GetCurrentSource();
            var serverHost = Source.Instance.GetApiUrl(source.Host);
            var result = $"{serverHost}/{method}";
            return result;
        }
    }
}
