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
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentOrderFragment : MvxFragment
    {
        public DocumentOrderViewModel DocumentOrderViewModel => (ViewModel as DocumentOrderViewModel);

        private string LogTag;

        private MvxListView _objListView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.DocumentOrderFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentOrderViewModel == null)
                objSingletonService.DocumentOrderViewModel = CMvvmCrossTools.LoadViewModel<DocumentOrderViewModel>();

            ViewModel = objSingletonService.DocumentOrderViewModel;

            var objAutoCompleteTextView = objView.FindViewById<MvxAutoCompleteTextView>(orderline.droid.Resource.Id.selectionarticlesearchbox);
            objAutoCompleteTextView.InputType = CTools.GetInputType((int)DocumentOrderViewModel.KeyboardSetting);
            objAutoCompleteTextView.RequestFocus();

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.documentorder_listview);
            _objListView.SmoothScrollbarEnabled = true;
            _objListView.SmoothScrollBy(1,1);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(DocumentOrderViewModel.LabelEdit);
            objMenu.Add(DocumentOrderViewModel.LabelDelete);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocumentdetail = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Documentdetail;

            if (strSelectedAction == DocumentOrderViewModel.LabelEdit)
            {
                DocumentOrderViewModel.PositionEditCommand.Execute(objDocumentdetail);
            }
            else if (strSelectedAction == DocumentOrderViewModel.LabelDelete)
            {
                DocumentOrderViewModel.PositionDeleteCommand.Execute(objDocumentdetail);
            }

            return base.OnContextItemSelected(objItem);
        }
    
    }
}