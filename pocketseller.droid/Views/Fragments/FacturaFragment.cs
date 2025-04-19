using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class FacturaFragment : BaseTabHostFragment
    {
        public FacturaViewModel FacturaViewModel => (ViewModel as FacturaViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.StockFragment, null);

            _objFragmentTabHost = objView.FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(Activity, ChildFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(FacturaAdress), FacturaViewModel.LabelFacturaAdress, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(FacturaDay), FacturaViewModel.LabelFacturaDay, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(FacturaWeek), FacturaViewModel.LabelFacturaWeek, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(FacturaDefect), FacturaViewModel.LabelFacturaDefect, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 8);

            return objView;
        }
    }
}