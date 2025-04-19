using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentInfoFragment : MvxFragment
    {
        public DocumentInfoViewModel DocumentInfoViewModel => (ViewModel as DocumentInfoViewModel);

        private string LogTag;

        private EditText _objEditText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentInfoFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentInfoViewModel == null)
                objSingletonService.DocumentInfoViewModel = CMvvmCrossTools.LoadViewModel<DocumentInfoViewModel>();

            ViewModel = objSingletonService.DocumentInfoViewModel;

            _objEditText = objView.FindViewById<EditText>(pocketseller.droid.Resource.Id.documentinfo_textedit_info);

            return objView;
        }

        public override void OnStart()
        {
            base.OnStart();
            _objEditText?.RequestFocus();
        }
    }
}