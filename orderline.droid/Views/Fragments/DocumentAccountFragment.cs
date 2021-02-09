using Android.OS;
using Android.Views;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentAccountFragment : MvxFragment
    {
        public DocumentAccountViewModel DocumentAccountViewModel => (ViewModel as DocumentAccountViewModel);

        private string LogTag;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var objView = this.BindingInflate(orderline.droid.Resource.Layout.DocumentAccountFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentAccountViewModel == null)
                objSingletonService.DocumentAccountViewModel = CMvvmCrossTools.LoadViewModel<DocumentAccountViewModel>();

            ViewModel = objSingletonService.DocumentAccountViewModel;

            return objView;
        }
    }
}