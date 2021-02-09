using Android.OS;
using Android.Views;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentAddressFragment : MvxFragment
    {
        public DocumentAddressViewModel DocumentAddressViewModel => (ViewModel as DocumentAddressViewModel);

        private string LogTag;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var objView = this.BindingInflate(orderline.droid.Resource.Layout.DocumentAddressFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentAddressViewModel == null)
                objSingletonService.DocumentAddressViewModel = CMvvmCrossTools.LoadViewModel<DocumentAddressViewModel>();

            ViewModel = objSingletonService.DocumentAddressViewModel;

            var objAutoCompleteTextView = objView.FindViewById<MvxAutoCompleteTextView>(orderline.droid.Resource.Id.selectionaddresssearchbox);
            //objAutoCompleteTextView.Context.SetTheme(orderline.droid.Resource.Style.AutoCompleteTextViewHoloDark);
            objAutoCompleteTextView.InputType = CTools.GetInputType((int)DocumentAddressViewModel.KeyboardSetting);

            objAutoCompleteTextView.RequestFocus();

            return objView;
        }
    }
}