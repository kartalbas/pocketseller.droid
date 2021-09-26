using System;
using System.Threading.Tasks;
using MvvmCross;
using orderline.core.Resources.Languages;
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
                    CTools.ShowToast(Language.IsNotActivated);
                    throw new UnauthorizedAccessException();
                }

                ReplaceAuthParameter(objRequest);

                var result = await GetResponse(objClient, objRequest);

                if(result.StatusDescription.Equals("Unauthorized"))
                {
                    throw new UnauthorizedAccessException();
                }

                return result;
            }
            catch (Exception exception)
            {
                CTools.ShowToast(exception.Message);
                throw;
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