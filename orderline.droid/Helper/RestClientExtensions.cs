using System;
using System.Threading.Tasks;
using MvvmCross;
using pocketseller.core;
using pocketseller.core.Services.Interfaces;
using RestSharp;

namespace pocketseller.droid.Helper
{
    public class RestClientWrapper
    {
        private static async Task<IRestResponse> GetResponse(RestClient objClient, IRestRequest objRequest)
        {
            var result = await Task.Run(() =>
            {
                var objTaskDone = new TaskCompletionSource<IRestResponse>();

                objClient.ExecuteAsync(objRequest, objResponse =>
                {
                    if (objResponse.ErrorException == null)
                    {
                        objTaskDone.SetResult(objResponse);
                    }
                    else
                    {
                        objTaskDone.SetException(objResponse.ErrorException);
                    }
                });

                return objTaskDone.Task;
                });

            return result;
        }

        public static async Task<IRestResponse> GetResponseAsync(RestClient objClient, IRestRequest objRequest)
        {
            objRequest.AddHeader("Authorization", "Bearer " + App.BackendToken);

            try
            {
                var result = await GetResponse(objClient, objRequest);
                return result;
            }
            catch (Exception)
            {
                App.BackendToken = await Mvx.IoCProvider.Resolve<IRestService>()?.GetToken();
                var result = await GetResponse(objClient, objRequest);
                return result;
            }
        }
    }
}