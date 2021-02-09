using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.ViewModels;
using pocketseller.core;
using pocketseller.core.Services.Interfaces;
using pocketseller.droid.CustomBindings;
using pocketseller.droid.Services;

namespace pocketseller.droid
{
    [Application(UiOptions = UiOptions.SplitActionBarWhenNarrow)]
    public class Setup : MvxAndroidSetup
    {
        protected override IMvxApplication CreateApp()
        {
            UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            Mvx.IoCProvider.RegisterSingleton<IBasicPlatformService>(new BasicPlatformService());
            Mvx.IoCProvider.RegisterSingleton<IMailService>(new EmailService());
            Mvx.IoCProvider.RegisterSingleton<IReportService>(new ReportService());
            Mvx.IoCProvider.RegisterSingleton<IPrintService>(new PrintService());
            return new App();
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry objRegistry)
        {
            objRegistry.RegisterCustomBindingFactory<EditText>("FocusText", textView => new MvxEditViewFocusChange(textView));
            base.FillTargetFactories(objRegistry);
        }
    }
}