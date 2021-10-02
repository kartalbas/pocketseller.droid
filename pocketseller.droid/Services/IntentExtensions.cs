using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acr.UserDialogs;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Java.Lang;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace pocketseller.droid.Services
{
    public static class IntentExtensions
    {
        private const string PackageNameFileProvider = "pocketseller.droid.fileprovider";
        private const string ApplicationType = "application/pdf";

        public static void AddAttachments(this Intent intent, IEnumerable<string> attachments)
        {
            try
            {
                if (attachments == null || !attachments.Any())
                {
                    Log.Warn("Intent.AddAttachments", "No attachments to attach.");
                    return;
                }

                IList<IParcelable> uris = new List<IParcelable>();
                foreach (var attachment in attachments)
                {
                    var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
                    var fileName = Path.GetFileNameWithoutExtension(attachment);
                    var extension = Path.GetExtension(attachment);

                    var targetFile = Path.Combine(activity.ApplicationContext.CacheDir.Path, $"{fileName}{extension}");

                    if (File.Exists(attachment))
                    {
                        File.Copy(attachment, targetFile, true);
                    }
                    else
                    {
                        Log.Warn("Intent.AddAttachments", "Unable to attach file '{0}', because it doesn't exist.", attachment);
                    }

                    if (File.Exists(targetFile))
                    {
                        fileName = Path.GetFileNameWithoutExtension(targetFile);
                        extension = Path.GetExtension(targetFile);
                        var javaFile = new Java.IO.File(activity.ApplicationContext.CacheDir, $"{fileName}{extension}");
                        var uri = FileProvider.GetUriForFile(activity.ApplicationContext, PackageNameFileProvider, javaFile);
                        uris.Add(uri);
                    }
                    else
                    {
                        Log.Warn("Intent.AddAttachments", "Unable to attach file '{0}', because it doesn't exist.", attachment);
                    }
                }

                if (uris.Any())
                {
                    intent.PutParcelableArrayListExtra(Intent.ExtraStream, uris);
                    intent.SetFlags(ActivityFlags.NoHistory);
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
            }
            catch (Exception exception)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Alert(exception?.Message, "ERROR");
            }
            catch (System.Exception exception)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Alert(exception?.Message, "ERROR");
            }
        }
    }
}