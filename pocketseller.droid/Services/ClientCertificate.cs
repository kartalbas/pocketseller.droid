using Java.Security;
using Java.Security.Cert;
using System.Collections.Generic;

namespace pocketseller.droid.Services
{
    public class ClientCertificate
    {
        public IPrivateKey PrivateKey { get; set; }

        public IReadOnlyCollection<X509Certificate> X509CertificateChain { get; set; }
    }
}

