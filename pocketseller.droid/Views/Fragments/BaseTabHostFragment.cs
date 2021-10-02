using Android.OS;
using Android.Support.V4.App;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;

namespace pocketseller.droid.Views.Fragments
{
    public class BaseTabHostFragment : MvxFragment
    {
        protected string LogTag { get; set; }

        protected MvxSubscriptionToken _objToken;
        protected FragmentTabHost _objFragmentTabHost;

        public BaseTabHostFragment()
        {
            LogTag = GetType().ToString();
            HasOptionsMenu = true;
        }

        protected void SubscribeMessenger()
        {
            _objToken = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<WorkingMessage>(OnWorking);
        }

        protected void UnSubscribeMessenger()
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<WorkingMessage>(_objToken);
            _objToken.Dispose();
            _objToken = null;
        }

        private void OnWorking(WorkingMessage objWorkingMessage)
        {
            Activity.RunOnUiThread(() =>
            {
                switch (objWorkingMessage.EWorkingAction)
                {
                    case EWorkingAction.ShowWorking:
                        if (_objFragmentTabHost != null)
                            _objFragmentTabHost.TabWidget.Enabled = false;
                        break;
                    case EWorkingAction.HideWorking:
                        if (_objFragmentTabHost != null)
                            _objFragmentTabHost.TabWidget.Enabled = true;
                        break;
                }
            });
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            SubscribeMessenger();
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroy()
        {
            UnSubscribeMessenger();
            base.OnDestroy();
        }        
    }
}