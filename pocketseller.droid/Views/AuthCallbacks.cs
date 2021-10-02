using MvvmCross;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;
using Firebase;
using Firebase.Auth;
using pocketseller.core.Resources.Languages;
using MvvmCross.Platforms.Android;
using System.Threading.Tasks;
using pocketseller.core.Services;
using Acr.UserDialogs;

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
                var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>();
                var restService = Mvx.IoCProvider.Resolve<IRestService>();

                var googleCred = await LoginView.FireAuth.SignInWithCredentialAsync(credential);
                var idToken = await googleCred.User.GetIdTokenAsync(false);

                var tuple = await restService.GetMobileToken(Username, Mobile, idToken.Token, Sourcename);

                if((string.IsNullOrEmpty(tuple.Item1) || string.IsNullOrEmpty(tuple.Item2)) && !tuple.Item3)
                {
                    CTools.ShowToast(Language.NotRegistered);
                    return;
                }

                LoginViewModel.SetLoginData(Sourcename, tuple.Item2, tuple.Item1, Username, tuple.Item3);

                if(tuple.Item3)
                {
                    var changed = await ChangePassword();
                    if(changed)
                        CTools.ShowToast(Language.Changed);
                    else
                        CTools.ShowToast(Language.NotRegistered);

                    LoginViewModel.SetLoginData(string.Empty, string.Empty, string.Empty, string.Empty, false);
                    LoginViewModel.ControlIsEnabled = true;
                    return;
                }

                await LoginViewModel.CheckLogin(false, true);

                CTools.ShowToast(Language.LoginSuccessFull);
                LoginViewModel.ControlIsEnabled = true;
                activity?.Activity?.Finish();
            }
            catch (Exception exception)
            {
                LoginViewModel.SetLoginData(string.Empty, string.Empty, string.Empty, string.Empty, false);
                LoginViewModel.ControlIsEnabled = true;
                CTools.ShowToast(exception.Message);
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

        private async Task<bool> ChangePassword()
        {
            try
            {
                var restService = Mvx.IoCProvider.Resolve<IRestService>();
                var dialogService = Mvx.IoCProvider.Resolve<IUserDialogs>();

                var changePassword = LoginViewModel.SettingService.Get<bool>(ESettingType.ChangePassword);
                if (changePassword)
                {
                    var result = await dialogService.PasswordChangeAsync(new PasswordChangeConfig
                        {
                            Message = Language.Password,
                            OkText = Language.Ok,
                            CancelText = Language.Cancel,
                            OldPasswordPlaceholder = Language.OldPassword,
                            NewPasswordPlaceholder = Language.NewPassword1,
                            NewPasswordConfirmPlaceholder = Language.NewPassword2
                        });

                    if (result.Ok)
                    {
                        if (result.NewPassword.Equals(result.NewPasswordConfirm))
                        {
                            var changed = await restService.ChangePassword(result.LoginText, result.OldPassword, result.NewPassword, result.NewPasswordConfirm);
                            return changed;
                        }
                    }
                }

                return false;

            }
            catch (Exception exception)
            {
                CTools.ShowToast(exception.Message);
                return false;
            }
        }
    }
}