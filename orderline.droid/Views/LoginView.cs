using System.Net;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;
using Firebase;
using Firebase.Auth;
using Java.Util.Concurrent;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon",
        Theme = "@style/Theme.Pocketsellertheme1",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public sealed class LoginView : MvxFragmentActivity
    {
        #region private members

        private LoginViewModel LoginViewModel => ViewModel as LoginViewModel;
        private bool _bWorking;
        private Button _objButtonExit;
        private Button _objButtonLogin;
        private MvxSubscriptionToken _objTokenWorkingMessage;

        #endregion

        #region Public and Protected methods

        internal static FirebaseApp FireApp;
        internal static FirebaseAuth FireAuth;
        
        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(orderline.droid.Resource.Layout.LoginView);
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;

            InitFirebaseAuth();
            RegisterEvents();
            ShowMainView();

            CTools.InitActionBar(ActionBar, LoginViewModel.LabelTitle);

            OrderSettings.Instance.CheckVKMustHigher = true; // setting disabled and fixed to true
            OrderSettings.Instance.CheckStock = true; // setting disabled and fixed to true


            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            string displayableVersion = $"{version}\r\n{buildDate:g}".Replace("/", ".");

            var objIdentification = FindViewById<TextView>(orderline.droid.Resource.Id.loginview_identification);
            objIdentification.Text = Mvx.IoCProvider.Resolve<IBasicPlatformService>()?.GetDeviceIdentification() + "\r\n" + displayableVersion;
        }

        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
               .SetApplicationId("1:569119279247:android:d3a019cbe06c8ab8ba88fa")
               .SetApiKey("AIzaSyDQ7btXxB2wNX97HWAiTEGfvpR4mZgV820")
               .Build();

            if (FireApp == null)
                FireApp = FirebaseApp.InitializeApp(this, options);

            FireAuth = FirebaseAuth.GetInstance(FireApp);

            FireAuth.UseAppLanguage();
        }

        private void ShowMainView()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessNetworkState }, 0);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BindPrintService) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.BindPrintService }, 0);
            }

            LoginViewModel.ShowMainViewCommand.Execute(null);
        }

        public override void OnBackPressed()
        {
            if (!_bWorking)
            {
                Finish();
                return;
            }

            base.OnBackPressed();
        }

        #endregion

        #region private methods

        private void RegisterEvents()
        {
            _objTokenWorkingMessage = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<WorkingMessage>(OnWorking);

            _objButtonExit = FindViewById<Button>(orderline.droid.Resource.Id.loginview_exit);
            _objButtonLogin = FindViewById<Button>(orderline.droid.Resource.Id.loginview_login);

            if (_objButtonExit != null)
                _objButtonExit.Click += (sender, e) => Finish();

            if (_objButtonLogin != null)
                _objButtonLogin.Click += (sender, e) =>
                {
                    //if (string.IsNullOrEmpty(LoginViewModel.Username))
                    //{
                    //    CTools.ShowToast(Language.LoginFailed + " -> Username?");
                    //    return;
                    //}
                    //else if (string.IsNullOrEmpty(LoginViewModel.Password))
                    //{
                    //    CTools.ShowToast(Language.LoginFailed + " -> Password?");
                    //    return;
                    //}
                    //else
                    //{
                        var instance = FirebaseAuth.GetInstance(FirebaseApp.GetInstance(FirebaseApp.DefaultAppName));
                        if (instance == null)
                        {
                            instance = new FirebaseAuth(FirebaseApp.GetInstance(FirebaseApp.DefaultAppName));
                        }

                        var providerInstance = PhoneAuthProvider.GetInstance(instance);
                        providerInstance.VerifyPhoneNumber("+41791288496", 60, TimeUnit.Seconds, CTools.CurrentActivity, new AuthCallbacks());

                        //LoginViewModel.SettingService.Set(ESettingType.LoginTime, DateTime.Now);
                        //Mvx.IoCProvider.Resolve<IRestService>()?.GetToken().ContinueWith(t => pocketseller.core.App.BackendToken = t.Result);
                        //CTools.ShowToast(Language.LoginSuccessFull);
                        //ShowMainView();
                        //Finish();
                    //}
                };
        }

        private void OnWorking(WorkingMessage objWorkingMessage)
        {
            RunOnUiThread(() =>
            {
                switch (objWorkingMessage.EWorkingAction)
                {
                    case EWorkingAction.ShowWorking:
                        _bWorking = true;
                        SetProgressBarIndeterminateVisibility(true);
                        break;
                    case EWorkingAction.HideWorking:
                        SetProgressBarIndeterminateVisibility(false);
                        _bWorking = false;
                        break;
                }
            });
        }

        #endregion
    }

    public class AuthCallbacks : PhoneAuthProvider.OnVerificationStateChangedCallbacks
    {

        public override async void OnVerificationCompleted(PhoneAuthCredential credential)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("OnVerificationCompleted:CODE: " + credential.SmsCode);
                var result = await LoginView.FireAuth.SignInWithCredentialAsync(credential);
                var idToken = await result.User.GetIdTokenAsync(false);
                System.Diagnostics.Debug.WriteLine("OnVerificationCompleted:TOKEN: " + idToken.Token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public override void OnCodeAutoRetrievalTimeOut(string language)
        {
            base.OnCodeAutoRetrievalTimeOut(language);
            System.Diagnostics.Debug.WriteLine("OnCodeAutoRetrievalTimeOut: " + language);
        }

        public override void OnVerificationFailed(FirebaseException exception)
        {
            System.Diagnostics.Debug.WriteLine("OnVerificationFailed: " + exception);
        }

        public override void OnCodeSent(string verificationId, PhoneAuthProvider.ForceResendingToken forceResendingToken)
        {
            try
            {
                base.OnCodeSent(verificationId, forceResendingToken);
                System.Diagnostics.Debug.WriteLine("OnCodeSent " + verificationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}