using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using orderline.core.Resources.Languages;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.droid.Views.Fragments;
using Firebase;
using Firebase.Auth;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/Theme.Pocketsellertheme1", ScreenOrientation = ScreenOrientation.Portrait)]
    public sealed class MainView : MvxFragmentActivity
    {
        #region private members

        private MainViewModel MainViewModel => (ViewModel as MainViewModel);
        private IDictionary<string, object> _cobjFragments;
        private DrawerLayout _objDrawer;
        private ActionBarDrawerToggleWrapper _objDrawerToggle;
        private ListView _objDrawerList;
        private EMenu _objCurrentMenu;
        private MvxSubscriptionToken _objTokenWorkingMessage;
        private MvxSubscriptionToken _objTokenTitleMessage;
        private bool _bWorking;

        #endregion

        #region Public and Protected methods

        //public static FirebaseApp app;
        //internal FirebaseAuth auth;

        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);

            //InitFirebaseAuth(); 
            
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(orderline.droid.Resource.Layout.MainView);
            SetFinishOnTouchOutside(false);

            SubscribeMessenger();
            CTools.InitActionBar(ActionBar, MainViewModel.LabelTitle);

            InitFragments();
            InitDrawer();

            ShowMenu(EMenu.Documents);
        }

        public override void OnBackPressed()
        {
            if (_bWorking)
                return;

            if (_objCurrentMenu == EMenu.Documents)
            {
                CTools.ShowDialog(this
                    , Language.Exit
                    , Language.ExitProgram
                    , Language.Exit
                    , (sender, args) => Finish()
                    , Language.Stay
                    , (sender, args) => { }
                    , ""
                    , null);

                return;
            }

            base.OnBackPressed();

            CorrigiateCurrentMenu();
        }

        #region Lifecycle call backs
        protected override void OnPostCreate(Bundle objSavedInstanceState)
        {
            base.OnPostCreate(objSavedInstanceState);
            _objDrawerToggle.SyncState();
        }

        protected override void OnDestroy()
        {
            UnSubscribeMessenger();
            base.OnDestroy();
        }

        #endregion  

        public override void OnConfigurationChanged(Configuration objNewConfig)
        {
            base.OnConfigurationChanged(objNewConfig);
            _objDrawerToggle.OnConfigurationChanged(objNewConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem objItem)
        {
            if (objItem.ItemId == Android.Resource.Id.Home)
            {
                if (_objDrawer.IsDrawerOpen(_objDrawerList))
                    _objDrawer.CloseDrawer(_objDrawerList);
                else
                    _objDrawer.OpenDrawer(_objDrawerList);
            }

            return base.OnOptionsItemSelected(objItem);
        }

        #endregion

        #region private methods

        //private void InitFirebaseAuth()
        //{
        //    var options = new FirebaseOptions.Builder()
        //       .SetApplicationId("1:569119279247:android:d3a019cbe06c8ab8ba88fa")
        //       .SetApiKey("AIzaSyDQ7btXxB2wNX97HWAiTEGfvpR4mZgV820")
        //       .Build();
        //    if (app == null)
        //        app = FirebaseApp.InitializeApp(this, options);

        //    auth = FirebaseAuth.GetInstance(app);
        //}
        
        private void SubscribeMessenger()
        {
            _objTokenWorkingMessage = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<WorkingMessage>(OnWorking);
            _objTokenTitleMessage = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<TitleMessage>(OnTitleChanged);
        }

        private void UnSubscribeMessenger()
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<WorkingMessage>(_objTokenWorkingMessage);
            _objTokenWorkingMessage.Dispose();
            _objTokenWorkingMessage = null;

            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<TitleMessage>(_objTokenTitleMessage);
            _objTokenTitleMessage.Dispose();
            _objTokenTitleMessage = null;
        }

        private void OnTitleChanged(TitleMessage objTitleMessage)
        {
            RunOnUiThread(() =>
            {
                switch (objTitleMessage.ETitleAction)
                {
                    case ETitleAction.Changed:
                        ActionBar.Title = MainViewModel.LabelTitle;
                        break;
                }
            });
        }

        private void OnWorking(WorkingMessage objWorkingMessage)
        {
            RunOnUiThread(() =>
            {
                switch (objWorkingMessage.EWorkingAction)
                {
                    case EWorkingAction.ShowWorking:
                        _objDrawer.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                        _bWorking = true;
                        SetProgressBarIndeterminateVisibility(true);
                        break;
                    case EWorkingAction.HideWorking:
                        _objDrawer.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                        SetProgressBarIndeterminateVisibility(false);
                        _bWorking = false;
                        break;
                }
            });
        }

        private void CorrigiateCurrentMenu()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                var objBackstackEntry = SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - 1);

                if (objBackstackEntry.Name == typeof (DocumentsFragment).Name)
                {
                    _objCurrentMenu = EMenu.Documents;
                }
                else if (objBackstackEntry.Name == typeof (QuotationsFragment).Name)
                {
                    _objCurrentMenu = EMenu.Quotations;
                }
                else if (objBackstackEntry.Name == typeof(StockFragment).Name)
                {
                    _objCurrentMenu = EMenu.Stock;
                }
                else if (objBackstackEntry.Name == typeof(FacturaFragment).Name)
                {
                    _objCurrentMenu = EMenu.Factura;
                }
                else if (objBackstackEntry.Name == typeof(DataInterfaceFragment).Name)
                {
                    _objCurrentMenu = EMenu.Dataflow;
                }
                else if (objBackstackEntry.Name == typeof (SettingFragment).Name)
                {
                    _objCurrentMenu = EMenu.Settings;
                }

                MainViewModel.LabelTitle = GetFragment(_objCurrentMenu).Item2;
                InvalidateOptionsMenu();
            }
        }

        private void ShowMenu(EMenu enmPosition)
        {
            ActionBar.RemoveAllTabs();
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            _objCurrentMenu = enmPosition;
            _objDrawerList.SetItemChecked((int)enmPosition, true);
            _objDrawer.CloseDrawer(_objDrawerList);

            var tobjResult = GetFragment(enmPosition);

            MainViewModel.LabelTitle = tobjResult.Item2;

            InvalidateOptionsMenu();

            if (tobjResult.Item1 != null)
            {
                SupportFragmentManager.BeginTransaction()
                    .Replace(orderline.droid.Resource.Id.main_drawer_content, tobjResult.Item1, tobjResult.Item1.GetType().Name)
                    .AddToBackStack(tobjResult.Item1.GetType().Name)
                    .Commit();
            }
        }

        private Tuple<MvxFragment, string> GetFragment(EMenu enmPosition)
        {
            MvxFragment objFragment = null;
            string strTitle = string.Empty;

            switch (enmPosition)
            {
                case EMenu.Documents:
                    objFragment = new DocumentsFragment { ViewModel = MainViewModel.DocumentsViewModel };
                    strTitle = ((DocumentsViewModel)objFragment.ViewModel).LabelTitle;
                    break;
                case EMenu.Quotations:
                    objFragment = new QuotationsFragment { ViewModel = MainViewModel.QuotationsViewModel };
                    strTitle = ((QuotationsViewModel)objFragment.ViewModel).LabelTitle;
                    break;
                case EMenu.Stock:
                    objFragment = new StockFragment { ViewModel = MainViewModel.StockViewModel };
                    strTitle = ((StockViewModel)objFragment.ViewModel).LabelTitle;
                    break;
                case EMenu.Factura:
                    objFragment = new FacturaFragment { ViewModel = MainViewModel.FacturaViewModel };
                    strTitle = ((FacturaViewModel)objFragment.ViewModel).LabelTitle;
                    break;
                case EMenu.Dataflow:
                    objFragment = (MvxFragment)_cobjFragments[typeof(DataInterfaceFragment).ToString()];
                    strTitle = ((DataInterfaceViewModel)objFragment.ViewModel).LabelTitle;
                    break;
                case EMenu.Settings:
                    objFragment = (MvxFragment)_cobjFragments[typeof(SettingFragment).ToString()];
                    strTitle = ((SettingViewModel)objFragment.ViewModel).LabelTitle;
                    break;
            }

            return new Tuple<MvxFragment, string>(objFragment, string.Format("${0}", strTitle));
        }

        private void InitDrawer()
        {
            _objDrawer = FindViewById<DrawerLayout>(orderline.droid.Resource.Id.main_drawer_layout);
            _objDrawer.SetScrimColor(GetColor(Android.Resource.Color.Transparent));
            _objDrawerList = FindViewById<ListView>(orderline.droid.Resource.Id.main_drawer_menu);
            _objDrawerList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                if ((EMenu) args.Position == EMenu.Logout)
                {
                    MainViewModel.ShowLoginViewCommand.Execute(null);
                    Finish();
                }

                ShowMenu((EMenu)args.Position);
            };

            _objDrawerToggle = new ActionBarDrawerToggleWrapper(this, _objDrawer, orderline.droid.Resource.Drawable.ic_drawer_light, orderline.droid.Resource.String.DrawerOpen, orderline.droid.Resource.String.DrawerClose);
            _objDrawerToggle.DrawerClosed += delegate
            {
                InvalidateOptionsMenu();
            };

            _objDrawerToggle.DrawerOpened += delegate
            {
                InvalidateOptionsMenu();
            };

            _objDrawer.AddDrawerListener(_objDrawerToggle);
        }

        private void InitFragments()
        {
            _cobjFragments = new Dictionary<string, object>
            {
                {
                    typeof (DocumentsFragment).ToString(),
                    new DocumentsFragment {ViewModel = MainViewModel.DocumentsViewModel}
                },
                {
                    typeof (QuotationsFragment).ToString(),
                    new QuotationsFragment {ViewModel = MainViewModel.QuotationsViewModel}
                },
                {
                    typeof (StockFragment).ToString(),
                    new StockFragment {ViewModel = MainViewModel.StockViewModel}
                },
                {
                    typeof (FacturaFragment).ToString(),
                    new FacturaFragment {ViewModel = MainViewModel.FacturaViewModel}
                },
                {
                    typeof (DataInterfaceFragment).ToString(),
                    new DataInterfaceFragment {ViewModel = MainViewModel.DataInterfaceViewModel}
                },
                {
                    typeof (SettingFragment).ToString(),
                    new SettingFragment {ViewModel = MainViewModel.SettingViewModel}
                }
            };
        }

        private enum EMenu
        {
            Documents = 0,
            Quotations = 1,
            Stock = 2,
            Factura = 3,
            Dataflow = 4,
            Settings = 5,
            Logout = 6
        }

        #endregion

    }
}