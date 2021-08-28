using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using MvvmCross;
using pocketseller.core.ModelConverter;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using RestSharp;
using RestSharp.Deserializers;
using Quotation = pocketseller.core.ModelsAPI.Quotation;
using pocketseller.core;

namespace pocketseller.droid.Helper
{
    public class CGmWebServices
    {
        private static CGmWebServices _objInstance;
        private static readonly object _objLock = new object();

        public static CGmWebServices Instance
        {
            get
            {
                if (_objInstance == null)
                    _objInstance = new CGmWebServices();

                return _objInstance;
            }
        }

        public void RefreshOrders(EOrderState enmState, string accountNumber, Action delStartAction, Action<ObservableCollection<Order>> delContinueAction, Action<Exception> delExceptionAction)
        {
            lock (_objLock)
            {
                delStartAction?.Invoke();

                GetDocuments(enmState, accountNumber)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            delContinueAction?.Invoke(new ObservableCollection<Order>(task.Result));
                        }
                        catch (Exception objException)
                        {
                            delExceptionAction?.Invoke(objException);
                        }
                    });
            }
        }

        public void RefreshOrders(EOrderState enmState, Action delStartAction, Action<ObservableCollection<Order>> delContinueAction, Action<Exception> delExceptionAction)
        {
            lock (_objLock)
            {
                delStartAction?.Invoke();

                GetDocuments(enmState)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            delContinueAction?.Invoke(new ObservableCollection<Order>(task.Result));
                        }
                        catch (Exception objException)
                        {
                            delExceptionAction?.Invoke(objException);
                        }
                    });
            }
        }

        public void RefreshOrders(EOrderState enmState, DateTime begin, DateTime end, Action delStartAction, Action<ObservableCollection<Order>> delContinueAction, Action<Exception> delExceptionAction)
        {
            lock (_objLock)
            {
                delStartAction?.Invoke();

                GetDocuments(enmState, begin, end)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            delContinueAction?.Invoke(new ObservableCollection<Order>(task.Result));
                        }
                        catch (Exception objException)
                        {
                            delExceptionAction?.Invoke(objException);
                        }
                    });
            }
        }

        public void RefreshQuotations(EOrderState enmState, Action delStartAction, Action<ObservableCollection<Quotation>> delContinueAction, Action<Exception> delExceptionAction)
        {
            lock (_objLock)
            {
                delStartAction?.Invoke();

                GetQuotations(enmState)
                    .ContinueWith(task =>
                    {
                        try
                        {
                            delContinueAction?.Invoke(new ObservableCollection<Quotation>(task.Result));
                        }
                        catch (Exception objException)
                        {
                            delExceptionAction?.Invoke(objException);
                        }
                    });
            }
        }

        public async Task<IRestResponse> DownloadFile(ESettingType enmResource, string strContentType)
        {
            try
            {
                if (CTools.Connected())
                {
                    var objSettings = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
                    var strResource = objSettings.Get<string>(enmResource);
                    var objRequest = new RestRequest(strResource, Method.GET);
                    objRequest.AddHeader("Accept", strContentType);

                    var objSource = Source.Instance.GetCurrentSource();
                    var strHost = Source.Instance.GetResourceUrl(objSource.Host);
                    var objClient = new RestClient(strHost);

                    var result = await RestClientWrapper.GetResponseAsync(new RestClient(strHost), objRequest);

                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                        throw new Exception(message);
                    };

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> GetLatestOrdernumber()
        {
            try
            {
                if (CTools.Connected())
                {
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetLatestOrdernumber, string.Empty);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);
                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                        throw new Exception(message);
                    };

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> ExportToErp(Order objDocument)
        {
            try
            {
                if (CTools.Connected())
                {
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestExportToErp, $"?orderNumber={objDocument.Docnumber}");
                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);
                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.ShowError(objException.Message);
                CErrorHandling.Log(objException);
                throw;
            }
        }

        public async Task<IRestResponse> ImportToErp(Order objDocument, ETargetDocumentType type)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{objDocument.Docnumber}/{(int)type}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestImportToErpAsDelivery, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);
                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw;
            }
        }

        public async Task<IRestResponse> ChangeDocumentState(EOrderState enmNewState, Order objDocument)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $"/{objDocument.Docnumber}/{(int) enmNewState}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestChangeDocumentState, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                        throw new Exception(message);
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> ChangeQuotationState(EOrderState enmNewState, Quotation objDocument)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $"/{objDocument.QuotationNr}/{(int)enmNewState}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestChangeQuotationState, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                    {
                        var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                        CTools.ShowMessage("Server", message);
                        throw new Exception(message);
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> SendQuotation(core.Models.Quotation objDocument)
        {
            try
            {
                if (CTools.Connected())
                {
                    var objRequest = CreateRequest( Method.POST, ESettingType.RestQuotationAddOrUpdate, string.Empty);

                    objRequest.AddHeader("Content-Encoding", "gzip");

                    var objOrderProxy = QuoToProxyQuotation.CreateQuotation(objDocument);
                    var strDeserializedContent = objRequest.JsonSerializer.Serialize(objOrderProxy);
                    objRequest.AddBody(GZipCompressor.CompressString(strDeserializedContent));

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> SendDocument(Document objDocument, ESettingType targetType)
        {
            try
            {
                if (CTools.Connected())
                {
                    var objRequest = CreateRequest( Method.POST, targetType, string.Empty);

                    var source = Source.Instance.GetCurrentSource();
                    objRequest.AddHeader("Content-Encoding", "gzip");
                    objRequest.AddHeader("externalUserId", source?.UserId.ToString());

                    var objOrderProxy = Converter.CreateOrder(source, objDocument);
                    var strDeserializedContent = objRequest.JsonSerializer.Serialize(objOrderProxy);

                    objRequest.AddBody(GZipCompressor.CompressString(strDeserializedContent));

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }
                    }

                    return result;
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                CTools.ShowMessage("Server", objException.Message);
                throw new Exception(objException.Message);
            }
        }

        public async Task<List<Order>> GetDocuments(EOrderState enmDocumentState)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int) enmDocumentState}";
                    var objRequest = CreateRequest( Method.GET, ESettingType.RestGetDocumentsByState, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }

                        var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(result);
                        return objDeserialized;
                    }
                    else
                    {
                        throw new Exception(result.ResponseStatus.ToString());
                    }
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<List<Order>> GetDocuments(EOrderState enmDocumentState, string accountNumber)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int)enmDocumentState}/{accountNumber}";
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetOrdersByAccountNumber, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }

                        var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(result);
                        return objDeserialized;
                    }
                    else
                    {
                        throw new Exception(result.ResponseStatus.ToString());
                    }
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<List<Order>> GetDocuments(EOrderState enmDocumentState, DateTime begin, DateTime end)
        {
            try
            {
                if (CTools.Connected())
                {
                    var sBegin = $"{begin.Year}-{begin.Month}-{begin.Day}";
                    var sEnd = $"{end.Year}-{end.Month}-{end.Day}";
                    var strSubUrl = $@"/{(int)enmDocumentState}/{sBegin}/{sEnd}";
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetDocumentsByStateAndTimeframe, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }

                        var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(result);
                        return objDeserialized;
                    }
                    else
                    {
                        throw new Exception(result.ResponseStatus.ToString());
                    }

                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<List<Quotation>> GetQuotations(EOrderState enmDocumentState)
        {
            try
            {
                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int) enmDocumentState}";
                    var objRequest = CreateRequest( Method.GET, ESettingType.RestGetQuotationsByState, strSubUrl);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    if (result.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                        {
                            var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(result.Content);
                            CTools.ShowMessage("Server", message);
                            throw new Exception(message);
                        }

                        var objDeserialized = new JsonDeserializer().Deserialize<List<Quotation>>(result);
                        return objDeserialized;
                    }
                    else
                    {
                        throw new Exception(result.ResponseStatus.ToString());
                    }
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> DownloadAndImport<TInput, TResult, T>(ESettingType enmResource, Func<TInput, TResult> delConverter, DataInterfaceViewModel objDataInterfaceViewModel)
        {
            try
            {
                if (CTools.Connected())
                {
                    var objRequest = CreateRequest(Method.GET, enmResource, string.Empty);

                    var result = await RestClientWrapper.GetResponseAsync(CreateClient(), objRequest);

                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Downloading, EventArgs.Empty);

                    try
                    {
                        if (result.ResponseStatus == ResponseStatus.Completed && result.Content.Length > 0)
                        {
                            if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
                                throw new Exception(result.StatusCode.ToString());

                            objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Deserializing, EventArgs.Empty);

                            var objDeserialized = new JsonDeserializer().Deserialize<TInput>(result);

                            objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Converting, EventArgs.Empty);

                            var cobjConverted = default(TResult);

                            if (delConverter != null)
                                cobjConverted = delConverter(objDeserialized);

                            objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Importing, EventArgs.Empty);

                            var objDataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();
                            objDataService.Recreate<T>(objDataService.PocketsellerConnection);

                            if (!Equals(cobjConverted, default(TResult)))
                                objDataService.PocketsellerConnection.InsertAll((IEnumerable)cobjConverted);
                            else
                                objDataService.PocketsellerConnection.InsertAll((IEnumerable)objDeserialized);

                            result.Content = Globals.TRUE;
                            objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Ready, EventArgs.Empty);
                        }
                        else
                        {
                            if (result.StatusDescription.Length > 0)
                                result.Content = result.StatusDescription;

                            objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Failed, EventArgs.Empty);
                        }

                        return result;
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException);
                        throw new Exception(objException.Message);
                    }
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        private RestClient CreateClient()
        {
            lock (_objLock)
            {
                try
                {
                    var objSource = Source.Instance.GetCurrentSource();
                    if(objSource == null)
                        throw new Exception("No source found, do you need to activate your pocketseller?");

                    var strHost = Source.Instance.GetApiUrl(objSource.Host);

                    var objClient = new RestClient(strHost)
                    {
                        Authenticator = new HttpBasicAuthenticator(RSACrypter.Encrypt(objSource.Username), RSACrypter.Encrypt(objSource.Password))
                    };

                    return objClient;
                }
                catch (Exception objException)
                {
                    CErrorHandling.Log(objException);
                    throw new Exception(objException.Message);
                }
            }
        }

        private IRestRequest CreateRequest(Method enmMethod, ESettingType enmResource, string strSubUrl)
        {
            lock (_objLock)
            {
                try
                {
                    var objSettings = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
                    var strResource = objSettings.Get<string>(enmResource);
                    var deviceId = Mvx.IoCProvider.Resolve<IBasicPlatformService>()?.GetDeviceIdentification();

                    if (!string.IsNullOrEmpty(strSubUrl))
                        strResource = $"{strResource}{strSubUrl}";

                    var objRequest = new RestRequest(strResource, enmMethod) { RequestFormat = DataFormat.Json };
                    objRequest.Timeout = 10 * 60 * 1000; // 10min
                    objRequest.AddHeader("Accept", "application/json");
                    objRequest.AddHeader("IMEI", RSACrypter.Encrypt(deviceId));

                    return objRequest;
                }
                catch (Exception objException)
                {
                    CErrorHandling.Log(objException);
                    throw new Exception(objException.Message);
                }
            }
        }
    }
}
