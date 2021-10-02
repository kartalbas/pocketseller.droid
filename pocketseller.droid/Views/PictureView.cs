
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Views;
using pocketseller.core.ViewModels;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/Theme.Pocketsellertheme1", ScreenOrientation = ScreenOrientation.Portrait)]
    public class PictureView : MvxActivity
    {
        public PictureViewModel PictureViewModel => ViewModel as PictureViewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(pocketseller.droid.Resource.Layout.PictureView);
            ActionBar.Hide();
        }
    }
}