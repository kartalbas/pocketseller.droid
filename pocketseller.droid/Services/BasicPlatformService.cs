using System;
using System.IO;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Views.InputMethods;
using MvvmCross;
using MvvmCross.Platforms.Android;
using pocketseller.core.Services.Interfaces;
using Environment = System.Environment;

namespace pocketseller.droid.Services
{
    public class BasicPlatformService : IBasicPlatformService
    {
        public void Finish()
        {
            Process.KillProcess(Process.MyPid());
        }

        public void ShowKeyboard()
        {
            var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var inputMethodManager = activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager?.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public void HideKeyboard()
        {
            var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var token = activity.CurrentFocus?.WindowToken;
            var inputMethodManager = activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager?.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            activity.Window.DecorView.ClearFocus();
        }
        
        public bool IsKeyboardShown()
        {
            var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var inputMethodManager = activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            var result = inputMethodManager?.IsAcceptingText;
            return result ?? false;
        }
        

        public void CreateFolderIfNotExists(string strFolder)
        {
            if (!string.IsNullOrEmpty(strFolder) && !Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
        }

        public string GetDeviceIdentification()
        {
            try
            {
                //return "02:00:00:00:00:00";
                var objActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
                var result = Settings.Secure.GetString(objActivity.ContentResolver, Settings.Secure.AndroidId);
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }        
        
        public void SaveText(string strFilename, string text)
        {
            var strDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            var strFilePath = Path.Combine(strDocumentsPath, strFilename);
            File.WriteAllText(strFilePath, text);
        }

        public string LoadText(string strFilename)
        {
            var strDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            var strFilePath = Path.Combine(strDocumentsPath, strFilename);
            return File.ReadAllText(strFilePath);
        }

        public string GetPersonalFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public string GetMyDocumentsFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public string GetExternalAbsolutePath()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }
    }
}