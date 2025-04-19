using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class QuotationsFragment : BaseTabHostFragment
    {
        public QuotationsViewModel QuotationsViewModel => (ViewModel as QuotationsViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentsFragment, null);

            _objFragmentTabHost = objView.FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(Activity, ChildFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(QuotationsNewFragment), QuotationsViewModel.LabelNewQuotations, pocketseller.droid.Resource.Drawable.documentsfragment_tab_newdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(QuotationsSentFragment), QuotationsViewModel.LabelSentQuotations, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 8);

            return objView;
        }
    }
}