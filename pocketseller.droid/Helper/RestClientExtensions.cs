using System;
using System.Threading.Tasks;
using MvvmCross;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using RestSharp;

namespace pocketseller.droid.Helper
{
    public class RestClientWrapper
    {
        public static async Task<RestResponse> GetResponseAsync(RestClient objClient, RestRequest objRequest)
        {
            try
            {
                var backendToken = Mvx.IoCProvider.Resolve<ISettingService>().Get<string>(ESettingType.BackendToken);

                if (string.IsNullOrEmpty(backendToken))
                {
                    CTools.ShowToast(Language.IsNotActivated);
                    throw new UnauthorizedAccessException();
                }

                ReplaceAuthParameter(objRequest, backendToken);

                var result = await objClient.ExecuteAsync(objRequest);

                if(result.StatusDescription.Equals("Unauthorized"))
                    throw new UnauthorizedAccessException();

                return result;
            }
            catch (Exception exception)
            {
                CTools.ShowToast(exception.Message);
                throw;
            }
        }

        private static void ReplaceAuthParameter(RestRequest objRequest, string backendToken)
        {
            var authParam = objRequest.Parameters.TryFind("Authorization");
            objRequest.Parameters.RemoveParameter(authParam);
            objRequest.AddHeader("Authorization", "Bearer " + backendToken);
        }
    }
}