using MvvmCross;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;
using Firebase;
using Firebase.Auth;
using orderline.core.Resources.Languages;
using pocketseller.core;
using pocketseller.core.Services;
using MvvmCross.Platforms.Android;

namespace pocketseller.droid.Views
{
    public class AuthCallbacks : PhoneAuthProvider.OnVerificationStateChangedCallbacks
    {
        private string Username { get; set; }
        public string Mobile { get; set; }
        public string Sourcename { get; set; }
        public LoginViewModel LoginViewModel { get; set; }

        public AuthCallbacks(string username, string mobile, string sourcename, LoginViewModel loginViewModel)
        {
            Username = username;
            Mobile = mobile;
            Sourcename = sourcename;
            LoginViewModel = loginViewModel;

        }

        public override async void OnVerificationCompleted(PhoneAuthCredential credential)
        {
            try
            {
                var result = await LoginView.FireAuth.SignInWithCredentialAsync(credential);
                var idToken = await result.User.GetIdTokenAsync(false);

                var rest = Mvx.IoCProvider.Resolve<IRestService>();
                var token = await rest.GetMobileToken(Username, Mobile, idToken.Token, Sourcename);
                if(string.IsNullOrEmpty(token))
                {
                    CTools.ShowToast(Language.NotRegistered);
                }

                App.BackendToken = token;
                App.SourceName = Sourcename;

                LoginViewModel.ControlIsEnabled = true;

                LoginViewModel.SettingService.Set(ESettingType.LoginTime, DateTime.Now);
                CTools.ShowToast(Language.LoginSuccessFull);

                LoginViewModel.ShowMainViewCommand.Execute(null);
                Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>()?.Activity?.Finish();
            }
            catch (Exception)
            {
                CTools.ShowToast(Language.NotRegistered);
            }
        }

        public override void OnCodeAutoRetrievalTimeOut(string language)
        {
            base.OnCodeAutoRetrievalTimeOut(language);
        }

        public override void OnVerificationFailed(FirebaseException exception)
        {
            CTools.ShowToast(exception.Message);
        }

        public override void OnCodeSent(string verificationId, PhoneAuthProvider.ForceResendingToken forceResendingToken)
        {
            try
            {
                base.OnCodeSent(verificationId, forceResendingToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}