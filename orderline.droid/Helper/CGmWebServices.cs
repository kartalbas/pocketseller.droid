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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var objSettings = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();
                    var strResource = objSettings.Get<string>(enmResource);
                    var objRequest = new RestRequest(strResource, Method.GET);
                    objRequest.AddHeader("Accept", strContentType);

                    var objSource = Source.Instance.GetCurrentSource();
                    var strHost = Source.Instance.GetResourceUrl(objSource.Host);
                    var objClient = new RestClient(strHost);

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Server", message);
                                throw new Exception(message);
                            }

                            objTaskDone.SetResult(objTask.Result);
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetLatestOrdernumber, string.Empty);

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Server", message);
                                throw new Exception(message);
                            }

                            objTaskDone.SetResult(objTask.Result);
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
            }
            catch (Exception objException)
            {
                CErrorHandling.Log(objException);
                throw new Exception(objException.Message);
            }
        }

        public async Task<IRestResponse> ImportToErp(Order objDocument, ETargetDocumentType type)
        {
            try
            {
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{objDocument.Docnumber}/{(int)type}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestImportToErpAsDelivery, strSubUrl);

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Server", message);
                                throw new Exception(message);
                            }

                            objTaskDone.SetResult(objTask.Result);
                            return objTaskDone.Task.Result;
                        });
                }
                else
                {
                    CTools.ShowMessage("App", Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var strSubUrl = $"/{objDocument.Docnumber}/{(int) enmNewState}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestChangeDocumentState, strSubUrl);

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Server", message);
                                throw new Exception(message);
                            }

                            objTaskDone.SetResult(objTask.Result);
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var strSubUrl = $"/{objDocument.QuotationNr}/{(int)enmNewState}";
                    var objRequest = CreateRequest(Method.POST, ESettingType.RestChangeQuotationState, strSubUrl);

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Server", message);
                                throw new Exception(message);
                            }

                            objTaskDone.SetResult(objTask.Result);
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var objRequest = CreateRequest( Method.POST, ESettingType.RestQuotationAddOrUpdate, string.Empty);

                    objRequest.AddHeader("Content-Encoding", "gzip");

                    var objOrderProxy = QuoToProxyQuotation.CreateQuotation(objDocument);
                    var strDeserializedContent = objRequest.JsonSerializer.Serialize(objOrderProxy);
                    objRequest.AddBody(GZipCompressor.CompressString(strDeserializedContent));

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                objTaskDone.SetResult(objTask.Result);
                            }

                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var objRequest = CreateRequest( Method.POST, targetType, string.Empty);

                    var source = Source.Instance.GetCurrentSource();
                    objRequest.AddHeader("Content-Encoding", "gzip");
                    objRequest.AddHeader("externalUserId", source?.UserId.ToString());

                    var objOrderProxy = Converter.CreateOrder(source, objDocument);
                    var strDeserializedContent = objRequest.JsonSerializer.Serialize(objOrderProxy);

                    objRequest.AddBody(GZipCompressor.CompressString(strDeserializedContent));

                    var objClient = CreateClient();
                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(objTask =>
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                objTaskDone.SetResult(objTask.Result);
                            }

                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<List<Order>>();

                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int) enmDocumentState}";
                    var objRequest = CreateRequest( Method.GET, ESettingType.RestGetDocumentsByState, strSubUrl);
                    var objClient = CreateClient();

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(delegate(Task<IRestResponse> objTask)
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(objTask.Result);
                                objTaskDone.SetResult(objDeserialized);
                            }
                            else
                            {
                                throw new Exception(objTask.Result.ResponseStatus.ToString());
                            }
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<List<Order>>();

                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int)enmDocumentState}/{accountNumber}";
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetOrdersByAccountNumber, strSubUrl);
                    var objClient = CreateClient();

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(delegate (Task<IRestResponse> objTask)
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(objTask.Result);
                                objTaskDone.SetResult(objDeserialized);
                            }
                            else
                            {
                                throw new Exception(objTask.Result.ResponseStatus.ToString());
                            }
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<List<Order>>();

                if (CTools.Connected())
                {
                    var sBegin = $"{begin.Year}-{begin.Month}-{begin.Day}";
                    var sEnd = $"{end.Year}-{end.Month}-{end.Day}";
                    var strSubUrl = $@"/{(int)enmDocumentState}/{sBegin}/{sEnd}";
                    var objRequest = CreateRequest(Method.GET, ESettingType.RestGetDocumentsByStateAndTimeframe, strSubUrl);
                    var objClient = CreateClient();

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(delegate (Task<IRestResponse> objTask)
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                var objDeserialized = new JsonDeserializer().Deserialize<List<Order>>(objTask.Result);
                                objTaskDone.SetResult(objDeserialized);
                            }
                            else
                            {
                                throw new Exception(objTask.Result.ResponseStatus.ToString());
                            }
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<List<Quotation>>();

                if (CTools.Connected())
                {
                    var strSubUrl = $@"/{(int) enmDocumentState}";
                    var objRequest = CreateRequest( Method.GET, ESettingType.RestGetQuotationsByState, strSubUrl);
                    var objClient = CreateClient();

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(delegate(Task<IRestResponse> objTask)
                        {
                            if (objTask.Result.ResponseStatus == ResponseStatus.Completed)
                            {
                                if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                {
                                    var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                    CTools.ShowMessage("Server", message);
                                    throw new Exception(message);
                                }

                                var objDeserialized = new JsonDeserializer().Deserialize<List<Quotation>>(objTask.Result);
                                objTaskDone.SetResult(objDeserialized);
                            }
                            else
                            {
                                throw new Exception(objTask.Result.ResponseStatus.ToString());
                            }
                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                if (CTools.Connected())
                {
                    var objRequest = CreateRequest(Method.GET, enmResource, string.Empty);
                    var objClient = CreateClient();

                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Downloading, EventArgs.Empty);

                    await objClient.GetResponseAsync(objRequest)
                        .ContinueWith(delegate(Task<IRestResponse> objTask)
                        {
                            try
                            {
                                if (objTask.Result.ResponseStatus == ResponseStatus.Completed && objTask.Result.Content.Length > 0)
                                {
                                    if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                                        throw new Exception(objTask.Result.StatusCode.ToString());

                                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Deserializing,EventArgs.Empty);

                                    var objDeserialized =new JsonDeserializer().Deserialize<TInput>(objTask.Result);

                                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Converting,EventArgs.Empty);

                                    var cobjConverted = default(TResult);

                                    if (delConverter != null)
                                        cobjConverted = delConverter(objDeserialized);

                                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Importing,EventArgs.Empty);

                                    var objDataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();
                                    objDataService.Recreate<T>(objDataService.PocketsellerConnection);

                                    if (!Equals(cobjConverted, default(TResult)))
                                        objDataService.PocketsellerConnection.InsertAll((IEnumerable)cobjConverted);
                                    else
                                        objDataService.PocketsellerConnection.InsertAll((IEnumerable)objDeserialized);

                                    objTask.Result.Content = Globals.TRUE;
                                    objTaskDone.SetResult(objTask.Result);
                                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Ready,EventArgs.Empty);
                                }
                                else
                                {
                                    if (objTask.Result.StatusDescription.Length > 0)
                                        objTask.Result.Content = objTask.Result.StatusDescription;

                                    objDataInterfaceViewModel.OnStatusUpdate(typeof(T).Name + " " + DataInterfaceViewModel.EState.Failed, EventArgs.Empty);
                                    objTaskDone.SetResult(objTask.Result);
                                }
                            }
                            catch (Exception objException)
                            {
                                CErrorHandling.Log(objException);
                                throw new Exception(objException.Message);
                            }

                        });
                }
                else
                {
                    throw new Exception(Language.NoInternet);
                }

                return objTaskDone.Task.Result;
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
