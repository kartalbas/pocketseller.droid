using Android.App;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Views;

namespace pocketseller.droid
{
    [Activity(
		Label = "pocketseller"
        , MainLauncher = true
        , Icon = "@drawable/icon"
		, Theme = "@style/Theme.Splash"
		, NoHistory = true
		, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(pocketseller.droid.Resource.Layout.SplashScreen)
        {
        }
    }
}