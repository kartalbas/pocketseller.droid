using System.Globalization;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.droid;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class SettingFragment : MvxFragment
    {
        public SettingViewModel SettingViewModel => (ViewModel as SettingViewModel);

        public override View OnCreateView(LayoutInflater objInflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(objInflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.SettingView, null);

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_LowerInputInPercent,
                Language.LowerInputInPercent,
                SettingViewModel.TextLowerInputInPercent.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextLowerInputInPercent = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_GreaterEKInPercent,
                Language.GreaterEKInPercent,
                SettingViewModel.TextGreaterEKInPercent.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextGreaterEKInPercent = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_OrderSearchMaxChar,
                Language.OrderSearchMaxChar,
                SettingViewModel.TextOrderSearchMaxChar.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextOrderSearchMaxChar = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_beginordernumber,
                Language.OrderNumberBegin,
                SettingViewModel.TextOrderNumberBegin.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextOrderNumberBegin = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_currentordernumber,
                Language.OrderNumberCurrent,
                SettingViewModel.TextOrderNumberCurrent.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextOrderNumberCurrent = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputDigitDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_maxposition,
                Language.MaxDocumentDetails,
                SettingViewModel.TextOrderMaxDocumentdetails.ToString(CultureInfo.InvariantCulture), (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextOrderMaxDocumentdetails = CParser.SafeParseInt(objInputText.Text);
                });

            CTools.CreateInputTextDialog(Activity,
                objView,
                objInflater,
                Resource.Id.settingview_pictureurl,
                Language.PictureUrl,
                SettingViewModel.TextPictureUrl, (sender, args) =>
                {
                    var objDialog = sender as AlertDialog;
                    if (objDialog == null) return;
                    var objInputText = objDialog.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputtext_edittext);
                    if (objInputText != null)
                        SettingViewModel.TextPictureUrl = objInputText.Text;
                });

            return objView;
        }
    }
}