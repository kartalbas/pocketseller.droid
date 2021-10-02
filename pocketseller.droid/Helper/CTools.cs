using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Support.V4.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MvvmCross;
using MvvmCross.Platforms.Android;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;

namespace pocketseller.droid.Helper
{
    public class CTools
    {
        private static readonly object LOCK = new object();

        public static Activity CurrentActivity => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
        
        public static void ShowDialog(Context objContext, string strTitle, string strMessage
            , string strButton1Title, EventHandler<DialogClickEventArgs> delButton1Action
            , string strButton2Title, EventHandler<DialogClickEventArgs> delButton2Action
            , string strButton3Title, EventHandler<DialogClickEventArgs> delButton3Action)
        {
            lock (LOCK)
            {
                var objBuilder = new AlertDialog.Builder(objContext);
                AlertDialog objDialog = objBuilder.Create();
                objDialog.SetTitle(strTitle);
                objDialog.SetMessage(strMessage);

                if (delButton1Action != null)
                    objDialog.SetButton(strButton1Title, delButton1Action);

                if (delButton2Action != null)
                    objDialog.SetButton2(strButton2Title, delButton2Action);

                if (delButton3Action != null)
                    objDialog.SetButton3(strButton3Title, delButton3Action);

                objDialog.Show();
            }
        }

        public static void SetTabHostTextSizes(FragmentTabHost objFragmentTabHost, float size)
        {
            for (var i = 0; i < objFragmentTabHost.TabWidget.ChildCount; i++)
            {
                var textView = (TextView)objFragmentTabHost.TabWidget.GetChildAt(i).FindViewById(Android.Resource.Id.Title);
                textView.SetTextSize(ComplexUnitType.Sp, size);
            }
        }

        public static void ShowMessage(string strTitle, string strMessage)
        {
            lock (LOCK)
            {
                CurrentActivity.RunOnUiThread(() =>
                {
                    var objBuilder = new AlertDialog.Builder(CurrentActivity);
                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetTitle(strTitle);
                    objDialog.SetMessage(strMessage);
                    objDialog.SetButton("OK", (sender, args) => { });
                    objDialog.Show();
                });
            }
        }

        public static void ShowToast(string strMessage)
        {
            CurrentActivity.RunOnUiThread(() => Toast.MakeText(CurrentActivity, strMessage, ToastLength.Long).Show());
        }

        public static InputTypes GetInputType(int iKeyboardSetting)
        {
            switch (iKeyboardSetting)
            {
                case (int)EKeyboardType.Normal:
                    return InputTypes.ClassText | InputTypes.TextFlagNoSuggestions;
                case (int)EKeyboardType.Phone:
                    return InputTypes.ClassPhone;
                case (int)EKeyboardType.Number:
                    return (InputTypes.ClassNumber | InputTypes.TextFlagNoSuggestions | InputTypes.NumberFlagDecimal | InputTypes.ClassText | InputTypes.NumberFlagSigned);
            }

            return InputTypes.ClassText;
        }

        public static void CreateInputDigitDialog(Context objContext, View objView, LayoutInflater objInflater, int iResourceTextView, string strTitle, string strCurrentValue, EventHandler<DialogClickEventArgs> delPositivAction)
        {
            CreateDialog(objContext, objView, objInflater, iResourceTextView, strTitle, strCurrentValue, delPositivAction, pocketseller.droid.Resource.Layout.InputDigit, pocketseller.droid.Resource.Id.inputdigit_edittext);
        }

        public static void CreateInputDateDialog(Context objContext, View objView, LayoutInflater objInflater, int iResourceTextView, string strTitle, string strCurrentValue, EventHandler<DialogClickEventArgs> delPositivAction)
        {
            CreateDialog(objContext, objView, objInflater, iResourceTextView, strTitle, strCurrentValue, delPositivAction, pocketseller.droid.Resource.Layout.InputDate, pocketseller.droid.Resource.Id.inputdate_edittext);
        }

        public static void CreateInputTextDialog(Context objContext, View objView, LayoutInflater objInflater, int iResourceTextView, string strTitle, string strCurrentValue, EventHandler<DialogClickEventArgs> delPositivAction)
        {
            CreateDialog(objContext, objView, objInflater, iResourceTextView, strTitle, strCurrentValue, delPositivAction, pocketseller.droid.Resource.Layout.InputText, pocketseller.droid.Resource.Id.inputtext_edittext);
        }

        public static void CreateDialog(Context objContext, View objView, LayoutInflater objInflater, int iResourceTextView, string strTitle, string strCurrentValue, EventHandler<DialogClickEventArgs> delPositivAction, int iLayout, int iResourceId)
        {
            var objTextCurrentNumber = objView.FindViewById<TextView>(iResourceTextView);
            objTextCurrentNumber.LongClick += (sender, args) =>
            {
                var objDialog = new AlertDialog.Builder(objContext);
                objDialog.SetTitle(strTitle);

                var objDialogView = objInflater.Inflate(iLayout, null);
                objDialog.SetView(objDialogView);

                var objEditText = objDialogView.FindViewById<EditText>(iResourceId);
                objEditText.Text = strCurrentValue;
                objDialog.SetPositiveButton(Language.Apply, delPositivAction);

                objDialog.Show();
            };
        }


        public static AlertDialog.Builder CreateDialog(Context objContext, LayoutInflater objInflater, string strTitle, string strCurrentValue, EventHandler<DialogClickEventArgs> delPositivAction, int iLayout, int iResourceId)
        {
            var objDialog = new AlertDialog.Builder(objContext);
            objDialog.SetTitle(strTitle);

            var objDialogView = objInflater.Inflate(iLayout, null);
            objDialog.SetView(objDialogView);

            var objEditText = objDialogView.FindViewById<EditText>(iResourceId);
            objEditText.Text = strCurrentValue;
            objDialog.SetPositiveButton(Language.Apply, delPositivAction);

            return objDialog;
        }

        public static bool Connected()
        {
            var objConnection = GetNetworkInfo(CurrentActivity);
            return objConnection != null && objConnection.IsConnected;
        }

        public static void EnableOrDisableView(View objListView, bool bEnabled)
        {
            CurrentActivity.RunOnUiThread(() =>
            {
                objListView.Enabled = bEnabled;
            });
        }

        public static void CreateTab(FragmentTabHost objFragmentTabHost, Type objFragmentType, string strLabel, int iDrawableId)
        {
            var objTabSpec = objFragmentTabHost.NewTabSpec(strLabel);
            objTabSpec = objTabSpec.SetIndicator(strLabel, CurrentActivity.GetDrawable(iDrawableId));
            objFragmentTabHost.AddTab(objTabSpec, Class.FromType(objFragmentType), null);
        }

        public static NetworkInfo GetNetworkInfo(Context objContext)
        {
            var connectivityManager = (ConnectivityManager)objContext.GetSystemService(Context.ConnectivityService);
            return connectivityManager.ActiveNetworkInfo;            
        }

        public static void InitActionBar(ActionBar objActionBar, string strTitle)
        {
            objActionBar.RemoveAllTabs();
            objActionBar.NavigationMode = ActionBarNavigationMode.Standard;
            objActionBar.ThemedContext.SetTheme(pocketseller.droid.Resource.Style.Theme_Pocketsellertheme1_Widget);
            objActionBar.SetDisplayHomeAsUpEnabled(false);
            objActionBar.SetHomeButtonEnabled(false);
            objActionBar.SetDisplayShowHomeEnabled(true);
            objActionBar.SetDisplayShowCustomEnabled(false);
            objActionBar.SetDisplayShowTitleEnabled(true);
            objActionBar.Title = strTitle;            
        }
    }
}