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
using pocketseller.core.Resources.Languages;
using System.Threading.Tasks;
using pocketseller.core;

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
        
        protected override async void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(pocketseller.droid.Resource.Layout.LoginView);
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;

            InitFirebaseAuth();
            RegisterEvents();
            CheckPermissions();
            await CheckLogin();

            CTools.InitActionBar(ActionBar, LoginViewModel.LabelTitle);

            OrderSettings.Instance.CheckVKMustHigher = true; // setting disabled and fixed to true
            OrderSettings.Instance.CheckStock = true; // setting disabled and fixed to true

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            var objIdentification = FindViewById<TextView>(pocketseller.droid.Resource.Id.loginview_identification);
            App.Version = $"{version}\r\n{buildDate.Day}.{buildDate.Month}.{buildDate.Year}".Replace("/", ".");
            objIdentification.Text = App.Version;
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

        private void CheckPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessNetworkState }, 0);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BindPrintService) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.BindPrintService }, 0);
            }
        }

        private async Task CheckLogin()
        {
            if (!await LoginViewModel.CheckLogin(false, true))
            {
                CTools.ShowToast(Language.LoginFailed);
                return;
            }

            CTools.ShowToast(Language.LoginSuccessFull);

            Finish();
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

            _objButtonExit = FindViewById<Button>(pocketseller.droid.Resource.Id.loginview_exit);
            _objButtonLogin = FindViewById<Button>(pocketseller.droid.Resource.Id.loginview_login);

            if (_objButtonExit != null)
                _objButtonExit.Click += (sender, e) => Finish();

            if (_objButtonLogin != null)
                _objButtonLogin.Click += async (sender, e) =>
                {
                    LoginViewModel.ControlIsEnabled = false;
                    var resultTuple = await LoginViewModel.GetMobile();
                    var sourcename = resultTuple.Item1;
                    var username = resultTuple.Item2;
                    var mobile = resultTuple.Item3;

                    if (string.IsNullOrEmpty(sourcename) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(mobile))
                    {
                        CTools.ShowToast(Language.IsNotActivated);
                        LoginViewModel.ControlIsEnabled = true;
                        return;
                    } 

                    var instance = FirebaseAuth.GetInstance(FirebaseApp.GetInstance(FirebaseApp.DefaultAppName));
                    if (instance == null)
                    {
                        instance = new FirebaseAuth(FirebaseApp.GetInstance(FirebaseApp.DefaultAppName));
                    }

                    var providerInstance = PhoneAuthProvider.GetInstance(instance);
                    providerInstance.VerifyPhoneNumber(mobile, 60, TimeUnit.Seconds, CTools.CurrentActivity, new AuthCallbacks(username, mobile, sourcename, LoginViewModel));
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
}