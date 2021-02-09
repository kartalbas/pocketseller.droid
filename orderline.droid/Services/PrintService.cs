using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Print;
using Java.Lang;
using MvvmCross;
using MvvmCross.Platforms.Android;
using pocketseller.core.Services.Interfaces;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Services
{
    public class PrintService : IPrintService
    {
        public void PrintNative(string strFile)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var objPrintManager = (PrintManager)currentActivity.GetSystemService(Context.PrintService);
            var objPrintAdapter = new GenericPrintAdapter(Application.Context, strFile);
            objPrintManager.Print($"JobFor{strFile}", objPrintAdapter, null);
        }

        public void PrintWithHpApp(string strFile)
        {
            try
            {
                var objFile = new Java.IO.File(strFile);
                var objIntent = new Intent("org.androidprinting.intent.action.PRINT");
                objIntent.AddCategory(Intent.CategoryDefault);
                objIntent.SetDataAndType(Android.Net.Uri.FromFile(objFile), "application/pdf");

                var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

                if (AppAvailable(currentActivity, objIntent))
                {
                    currentActivity.StartActivityForResult(objIntent, 1);
                }

                throw new Exception("HP ePrint not found!");
            }
            catch (Exception exception)
            {
                throw new Exception($"Printing with HP APP failed! Error: {exception.Message}");
            }
        }

        private bool AppAvailable(Activity activity, Intent intent)
        {
            var packageManager = activity.PackageManager;
            var listPackages = packageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return listPackages.Count > 0;
        }
    }
}