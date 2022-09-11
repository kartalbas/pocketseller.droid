using System.Linq;
using Java.Security;
using Java.Security.Cert;
using Javax.Net.Ssl;
using Xamarin.Android.Net;

namespace pocketseller.droid.Services
{
    public class AndroidHttpsClientHander : AndroidClientHandler
    {
        private readonly ClientCertificate clientCertificate;

        public AndroidHttpsClientHander(ClientCertificate clientCertificate)
        {
            this.clientCertificate = clientCertificate;

            var trustManagerFactory = TrustManagerFactory
                .GetInstance(TrustManagerFactory.DefaultAlgorithm);

            trustManagerFactory.Init((KeyStore)null);

            var x509trustManager = trustManagerFactory
                .GetTrustManagers()
                .OfType<IX509TrustManager>()
                .FirstOrDefault();

            var acceptedIssuers = x509trustManager.GetAcceptedIssuers();

            TrustedCerts = clientCertificate.X509CertificateChain
                .Concat(acceptedIssuers)
                .ToList<Certificate>();
        }

        protected override KeyStore ConfigureKeyStore(KeyStore keyStore)
        {
            keyStore = KeyStore.GetInstance("PKCS12");

            keyStore.Load(null, null);

            keyStore.SetKeyEntry("privateKey", clientCertificate.PrivateKey,
                null, clientCertificate.X509CertificateChain.ToArray());

            if (TrustedCerts?.Any() == false)
                return keyStore;

            for (var i = 0; i < TrustedCerts.Count; i++)
            {
                var trustedCert = TrustedCerts[i];

                if (trustedCert == null)
                    continue;

                keyStore.SetCertificateEntry($"ca{i}", trustedCert);
            }

            return keyStore;
        }

        protected override KeyManagerFactory ConfigureKeyManagerFactory(KeyStore keyStore)
        {
            var keyManagerFactory = KeyManagerFactory.GetInstance("x509");
            keyManagerFactory.Init(keyStore, null);
            return keyManagerFactory;
        }

        protected override TrustManagerFactory ConfigureTrustManagerFactory(KeyStore keyStore)
        {
            var trustManagerFactory = TrustManagerFactory.GetInstance(TrustManagerFactory.DefaultAlgorithm);
            trustManagerFactory.Init(keyStore);
            return trustManagerFactory;
        }
    }
}

