using System.Net.Http;
using Android.App;

namespace pocketseller.core.Services.Interfaces
{
    public interface ICertificationService
    {
        void SetPrivateKeyFromUser(Activity activity);
        void SetCertificateChain();
        HttpClientHandler GetAuthAndroidClientHander();
        string GetCertificationDetailsError();
        void SetPrivateKey();
    }
}

