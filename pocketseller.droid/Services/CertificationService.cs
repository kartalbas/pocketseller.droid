﻿using System;
using Android.App;
using Android.Security;
using System.Net.Http;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.droid.Services
{
    public class CertificationService : ICertificationService
    {
        public ClientCertificate ClientCertificate { get; set; }

        public CertificationService()
        {
            ClientCertificate = new ClientCertificate();
        }

        public void SetPrivateKeyFromUser(Activity activity)
        {
            try
            {
                KeyChain.ChoosePrivateKeyAlias(
                    activity: activity,
                    response: new PrivateKeyCallback(this),
                    keyTypes: new string[] { "RSA" },
                    issuers: null,
                    uri: null,
                    alias: pocketseller.core.App.CertName);

                SetPrivateKey();

                if (ClientCertificate.PrivateKey != null)
                    return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void SetCertificateChain()
        {
            try
            {
                ClientCertificate.X509CertificateChain = KeyChain.GetCertificateChain(Android.App.Application.Context, pocketseller.core.App.CertName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public HttpClientHandler GetAuthAndroidClientHander()
        {
            try
            {
                var result = new AndroidHttpsClientHander(ClientCertificate);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string GetCertificationDetailsError()
        {
            if (ClientCertificate?.X509CertificateChain == null)
                return $"CertificationChainEmpty";

            if (ClientCertificate?.PrivateKey == null)
                return $"PrivateKeyIsNull";

            return null;
        }

        public void SetPrivateKey()
        {
            try
            {
                ClientCertificate.PrivateKey = KeyChain.GetPrivateKey(Android.App.Application.Context, pocketseller.core.App.CertName);
            }
            catch (KeyChainException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}

