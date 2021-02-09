using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/Theme.Pocketsellertheme1", WindowSoftInputMode = SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StockDocumentView : MvxFragmentActivity
    {
        #region Member variables

        public StockDocumentViewModel StockDocumentViewModel => (ViewModel as StockDocumentViewModel);

        #endregion

        #region Public methods

        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            SetContentView(orderline.droid.Resource.Layout.StockDocumentView);
            CTools.InitActionBar(ActionBar, StockDocumentViewModel.LabelTitle);
        }


        #endregion
    }
}