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
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;

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

        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(orderline.droid.Resource.Layout.LoginView);
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;

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
                    if (LoginViewModel.Login)
                    {
                        CTools.ShowToast(Language.LoginSuccessFull);
                        ShowMainView();
                        Finish();
                    }
                    else
                    {
                        CTools.ShowToast(Language.LoginFailed);
                    }
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