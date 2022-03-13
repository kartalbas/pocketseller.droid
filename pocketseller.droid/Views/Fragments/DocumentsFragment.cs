using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentsFragment : BaseTabHostFragment
    {
        public DocumentsViewModel DocumentsViewModel => (ViewModel as DocumentsViewModel);

        private Intent _objIntentDocumentView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentsFragment, null);

            _objFragmentTabHost = objView.FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(Activity, ChildFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(DocumentsNewFragment), DocumentsViewModel.LabelNewOrder, pocketseller.droid.Resource.Drawable.documentsfragment_tab_newdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 8);

            _objIntentDocumentView = new Intent(Activity, typeof(DocumentView));

            Activity.InvalidateOptionsMenu();

            return objView;
        }

        public override void OnCreateOptionsMenu(IMenu objMenu, MenuInflater inflater)
        {
            objMenu.Clear();

            objMenu.Add("")
                .SetIcon(pocketseller.droid.Resource.Drawable.ic_action_new_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnNewActionClicked))
                .SetShowAsAction(ShowAsAction.Always);

            base.OnCreateOptionsMenu(objMenu, inflater);
        }

        private bool OnNewActionClicked(IMenuItem objMenuItem)
        {
            DocumentsViewModel.InitDoucment();
            StartActivity(_objIntentDocumentView);
            return true;
        }
    }
}