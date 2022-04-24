using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class StockFragment : BaseTabHostFragment
    {
        public StockViewModel StockViewModel => (ViewModel as StockViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.StockFragment, null);

            _objFragmentTabHost = objView.FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(Activity, ChildFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(StockToPrintFragment), StockViewModel.LabelComission, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToDeliveryFragment), StockViewModel.LabelReady, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToFacturaFragment), StockViewModel.LabelDelivery, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToDeleteFragment), StockViewModel.LabelFactura, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(StockToCanceldFragment), StockViewModel.LabelToCanceled, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 6);

            return objView;
        }
    }
}