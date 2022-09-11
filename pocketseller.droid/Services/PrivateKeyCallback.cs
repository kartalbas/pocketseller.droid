using System;
using Android.Security;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.droid.Services
{
    public class PrivateKeyCallback : Java.Lang.Object, IKeyChainAliasCallback
    {
        private readonly ICertificationService certificationService;

        public PrivateKeyCallback(ICertificationService certificationService) => this.certificationService = certificationService;

        public void Alias(string alias)
        {
            if (alias != pocketseller.core.App.CertName)
                return;

            certificationService.SetPrivateKey();
        }
    }
}

