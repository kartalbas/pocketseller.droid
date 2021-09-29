using MvvmCross;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;
using Firebase;
using Firebase.Auth;
using orderline.core.Resources.Languages;
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
                var tuple = await rest.GetMobileToken(Username, Mobile, idToken.Token, Sourcename);
                if(string.IsNullOrEmpty(tuple.Item1) || string.IsNullOrEmpty(tuple.Item2))
                {
                    CTools.ShowToast(Language.NotRegistered);
                }

                LoginViewModel.SetLoginData(Sourcename, tuple.Item2, tuple.Item1, Username, tuple.Item3);

                LoginViewModel.ControlIsEnabled = true;

                CTools.ShowToast(Language.LoginSuccessFull);

                await LoginViewModel.CheckLogin(false, true);

                Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>()?.Activity?.Finish();
            }
            catch (Exception)
            {
                LoginViewModel.SetLoginData(string.Empty, string.Empty, string.Empty, string.Empty, false);
                CTools.ShowToast(Language.NotRegistered);
                LoginViewModel.ControlIsEnabled = true;
            }
        }

        public override void OnCodeAutoRetrievalTimeOut(string language)
        {
            base.OnCodeAutoRetrievalTimeOut(language);
        }

        public override void OnVerificationFailed(FirebaseException exception)
        {
            CTools.ShowToast(exception.Message);
            LoginViewModel.ControlIsEnabled = true;
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
                LoginViewModel.ControlIsEnabled = true;
            }
        }
    }
}