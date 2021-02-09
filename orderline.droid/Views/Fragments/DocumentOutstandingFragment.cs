using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.Models;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentOutstandingFragment : MvxFragment
    {
        public DocumentOutstandingViewModel DocumentOutstandingViewModel => (ViewModel as DocumentOutstandingViewModel);
        private MvxListView _objListView;

        private string LogTag;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.DocumentOutstandingFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentOutstandingViewModel == null)
                objSingletonService.DocumentOutstandingViewModel = CMvvmCrossTools.LoadViewModel<DocumentOutstandingViewModel>();

            ViewModel = objSingletonService.DocumentOutstandingViewModel;

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.documentoutstandingpayments_listview);
            RegisterForContextMenu(_objListView);
            
            DocumentOutstandingViewModel.ShowOoutstandingPayments();

            return objView;
        }
        
        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(DocumentOutstandingViewModel.LabelPayed);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as OpenPayment;
            
            if (strSelectedAction == DocumentOutstandingViewModel.LabelPayed)
            {
                DocumentOutstandingViewModel.MarkAsPayedCommand.Execute(objDocument);
            }
            
            return base.OnContextItemSelected(objItem);
        }
    }
}