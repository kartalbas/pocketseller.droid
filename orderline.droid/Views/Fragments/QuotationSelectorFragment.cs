using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ViewModels;

namespace pocketseller.droid.Views.Fragments
{
    [MvxDialogFragmentPresentation]
    [Register(nameof(QuotationSelectorFragment))]
    public sealed class QuotationSelectorFragment : MvxDialogFragment<QuotationSelectorViewModel>
    {
        #region Member variables

        public QuotationSelectorViewModel QuotationSelectorViewModel => ViewModel;

        private MvxSubscriptionToken _objToken;
        private MvxListView _objListView;
        private string LogTag;
        private View _view;
        private Button _buttonCancel;
        private Button _buttonOK;

        #endregion

        #region Public methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _view = this.BindingInflate(orderline.droid.Resource.Layout.QuotationSelectorFragment, null);
            _buttonCancel = _view.FindViewById<Button>(orderline.droid.Resource.Id.quotationselector_cancel);
            _buttonCancel.Click += (sender, e) => RestoreAndExit();

            _buttonOK = _view.FindViewById<Button>(orderline.droid.Resource.Id.quotationselector_ok);
            _buttonOK.Click += (sender, e) => TakeOverAndExit();

            LogTag = GetType().ToString();

            _objToken = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<DocumentdetailMessage>(OnDocumentdetailAction);
            _objListView = _view.FindViewById<MvxListView>(orderline.droid.Resource.Id.quotationselector_listview);

            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            return _view;
        }

        #endregion

        #region Lifecycle events

        public override void OnStart()
        {
            base.OnStart();

            if (Dialog != null)
            {
                Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

                var objAttributs = Dialog.Window.Attributes;
                objAttributs.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                objAttributs.Y = 20;
                Dialog.Window.SetWindowAnimations(orderline.droid.Resource.Style.DialogAnimation);
            }
        }

        public override void OnPause()
        {
            base.OnPause();

            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<DocumentdetailMessage>(_objToken);
            _objToken.Dispose();
            _objToken = null;
        }


        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            RestoreAndExit();
        }

        #endregion

        #region Private methods

        private void OnDocumentdetailAction(DocumentdetailMessage objDocumentdetailMessage)
        {
            switch (objDocumentdetailMessage.EDocumentdetailAction)
            {
                case EDocumentdetailAction.Exit:
                    Dismiss();
                    break;
                case EDocumentdetailAction.Stay:
                    break;
            }
        }

        private void TakeOverAndExit()
        {
            QuotationSelectorViewModel.TakeOverCommand.Execute(null);
            RestoreAndExit();
        }

        private void RestoreAndExit()
        {
            Dismiss();
        }

        #endregion

    }
}