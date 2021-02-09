using System;
using System.Threading.Tasks;
using RestSharp;

namespace pocketseller.droid.Helper
{
    public static class RestClientExtensions
    {
        private static Task<T> SelectAsync<T>(this RestClient objClient, IRestRequest objRequest, Func<IRestResponse, T> objSelector)
        {
            var objTaskDone = new TaskCompletionSource<T>();

            objClient.ExecuteAsync(objRequest, objResponse =>
            {
                if (objResponse.ErrorException == null)
                {
                    objTaskDone.SetResult(objSelector(objResponse));
                }
                else
                {
                    objTaskDone.SetException(objResponse.ErrorException);
                }
            });

            return objTaskDone.Task;
        }

        public static Task<string> GetContentAsync(this RestClient objClient, IRestRequest objRequest)
        {
            return objClient.SelectAsync(objRequest, response => response.Content);
        }

        public static Task<IRestResponse> GetResponseAsync(this RestClient objClient, IRestRequest objRequest)
        {
            return objClient.SelectAsync(objRequest, response => response);
        }
    }
}