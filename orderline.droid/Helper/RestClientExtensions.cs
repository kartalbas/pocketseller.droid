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
            try
            {
                if (string.IsNullOrEmpty(App.BackendToken))
                {
                    App.BackendToken = await Mvx.IoCProvider.Resolve<IRestService>()?.GetToken();
                }

                ReplaceAuthParameter(objRequest);

                var result = await GetResponse(objClient, objRequest);

                if(result.StatusDescription.Equals("Unauthorized"))
                {
                    throw new Exception("Unauthorized");
                }

                return result;
            }
            catch (Exception)
            {
                App.BackendToken = await Mvx.IoCProvider.Resolve<IRestService>()?.GetToken();
                ReplaceAuthParameter(objRequest);
                var result = await GetResponse(objClient, objRequest);
                return result;
            }
        }

        private static void ReplaceAuthParameter(IRestRequest objRequest)
        {
            var authParam = objRequest.Parameters.Find(p => p.Name.Equals("Authorization"));
            objRequest.Parameters.Remove(authParam);
            objRequest.AddHeader("Authorization", "Bearer " + App.BackendToken);
        }
    }
}