using pocketseller.core.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using MvvmCross.Plugin.Messenger;
using orderline.core.ModelsPS;
using MvvmCross;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using orderline.core.ModelsAPI;
using System.Text;
using orderline.core.Tools;

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
        }

        public async Task<IList<Stock>> GetAllStocks()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetLoginData().Item2);
                var methodUrl = GetPocketsellerHost("GetAllStocks");
                var response = await client.GetAsync(new Uri(methodUrl));
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<IList<Stock>>(content);
                    return result;
                }

                return new List<Stock>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FacturaData> GetFacturaData(int orderNumber)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetLoginData().Item2);
                var methodUrl = GetPocketsellerHost("GetFacturaData");
                var serverUri = new Uri($"{methodUrl}/{orderNumber}");
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

        public async Task<bool> SendMail(string from, string to, string subject, string body)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetLoginData().Item2);
                var methodUrl = GetMailHost("SendMail");
                var serverUri = new Uri($"{methodUrl}");

                using (var request = new HttpRequestMessage(HttpMethod.Post, serverUri))
                {
                    var json = JsonConvert.SerializeObject(new MailMessage
                    {
                        From = "",
                        To = to,
                        Subject = subject,
                        Body = body
                    });

                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<bool>(content);
                            return result;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IList<MailResponse>> GetMails()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetLoginData().Item2);
                var methodUrl = GetMailHost("GetMails");
                var serverUri = new Uri($"{methodUrl}");
                var response = await client.GetAsync(serverUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<IList<MailResponse>>(content);
                    return result;
                }

                return new List<MailResponse>();
            }
            catch (Exception)
            {
                return new List<MailResponse>();
            }
        }

        public async Task<bool> DeleteMail(string messageId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetLoginData().Item2);
                var methodUrl = GetMailHost("DeleteMail");
                var serverUri = new Uri($"{methodUrl}?messageId={messageId}");

                using (var request = new HttpRequestMessage(HttpMethod.Post, serverUri))
                {
                    using (var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<bool>(content);
                            return result;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IList<EMails>> GetOpMails()
        {
            try
            {
                var loginData = GetLoginData();
                if (string.IsNullOrEmpty(loginData.Item1) || string.IsNullOrEmpty(loginData.Item2))
                    return new ObservableCollection<EMails>();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginData.Item2);
                var methodUrl = GetPocketsellerHost("GetOpMails");
                var response = await client.GetAsync(new Uri(methodUrl));
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var mails = JsonConvert.DeserializeObject<IList<string>>(content);
                    var result = new List<EMails>();
                    var index = 1;
                    foreach(var mail in mails)
                    {
                        result.Add(new EMails
                        {
                            Id = index++,
                            Mail = mail,
                            TimeStamp = DateTime.Now
                        });
                    }

                    return result;
                }

                return new ObservableCollection<EMails>();
            }
            catch (Exception)
            {
                return new ObservableCollection<EMails>();
            }
        }

        public async Task<bool> Test()
        {
            try
            {
                var loginData = GetLoginData();
                if (string.IsNullOrEmpty(loginData.Item1) || string.IsNullOrEmpty(loginData.Item2))
                    return false;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginData.Item2);
                var methodUrl = GetLoginHost("LoginTest", loginData.Item1);
                var response = await client.GetAsync(new Uri(methodUrl));
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<bool>(content);
                    return result;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Tuple<string, string>> GetMobileToken(string username, string mobile, string token, string sourcename)
        {
            try
            {
                var deviceId = Mvx.IoCProvider.Resolve<IBasicPlatformService>()?.GetDeviceIdentification();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("username", Base64Tools.Base64Encode(username));
                client.DefaultRequestHeaders.Add("mobile", Base64Tools.Base64Encode(mobile));
                client.DefaultRequestHeaders.Add("token", token);
                var methodUrl = GetLoginHost("GetMobileToken", sourcename);
                var response = await client.GetAsync(new Uri(methodUrl));
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var jwt = JObject.Parse(content)["token"].ToString();
                    var id = JObject.Parse(content)["id"].ToString();
                    return new Tuple<string, string>(id, jwt);
                }

                return new Tuple<string, string>(string.Empty, string.Empty);
            }
            catch (Exception)
            {
                return new Tuple<string, string>(string.Empty, string.Empty);
            }
        }

        public async Task<string> GetMobile(string username, string password, string sourcename)
        {
            try
            {
                var deviceId = Mvx.IoCProvider.Resolve<IBasicPlatformService>()?.GetDeviceIdentification();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("username", Base64Tools.Base64Encode(username));
                client.DefaultRequestHeaders.Add("password", Base64Tools.Base64Encode(password));
                var methodUrl = GetLoginHost("GetMobile", sourcename);
                var response = await client.GetAsync(new Uri(methodUrl));
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetMailHost(string method)
        {
            var source = Source.Instance.GetCurrentSource();
            var serverHost = Source.Instance.GetMailUrl(source.Host);
            var result = $"{serverHost}/{method}";
            return result;
        }

        private string GetPocketsellerHost(string method)
        {
            var source = Source.Instance.GetCurrentSource();
            var serverHost = Source.Instance.GetApiUrl(source.Host);
            var result = $"{serverHost}/{method}";
            return result;
        }

        private string GetLoginHost(string method, string sourcename)
        {
            var source = Source.Instance.GetCurrentSource(sourcename);
            var serverHost = Source.Instance.GetLoginUrl(source.Host);
            var result = $"{serverHost}/{method}";
            return result;
        }

        private Tuple<string, string> GetLoginData()
        {
            var backendToken = _settingService.Get<string>(ESettingType.BackendToken);
            var sourceName = _settingService.Get<string>(ESettingType.SourceName);
            return new Tuple<string, string>(sourceName, backendToken);
        }
    }
}
