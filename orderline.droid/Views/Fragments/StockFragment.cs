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
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.StockFragment, null);

            _objFragmentTabHost = objView.FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(Activity, ChildFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(StockToPrintFragment), StockViewModel.LabelToPrint, orderline.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToDeliveryFragment), StockViewModel.LabelToDelivery, orderline.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToFacturaFragment), StockViewModel.LabelToFactura, orderline.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(ImportToDeleteFragment), StockViewModel.LabelToDelete, orderline.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(StockToCanceldFragment), StockViewModel.LabelToCanceled, orderline.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 6);

            return objView;
        }
    }
}