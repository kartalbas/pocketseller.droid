using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class StockToCanceldFragment : BaseTabWidgetFragment
    {
        public StockToCancelViewModel StockToCancelViewModel => (ViewModel as StockToCancelViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.StockToPrintFragment, null);

            if (SingletonService.StockToCancelViewModel == null)
                SingletonService.StockToCancelViewModel = CMvvmCrossTools.LoadViewModel<StockToCancelViewModel>();

            ViewModel = SingletonService.StockToCancelViewModel;
            EnablePrintButton = false;
            RefreshState = EOrderState.CANCELED;
            StockToCancelViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoprint_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View objView, IContextMenuContextMenuInfo objMenuInfo)
        {
            base.OnCreateContextMenu(objMenu, objView, objMenuInfo);
            objMenu.Add(StockToCancelViewModel.LabelShow);
            objMenu.Add(StockToCancelViewModel.LabelPutBack);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            return true;
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;

            if (strSelectedAction == StockToCancelViewModel.LabelPutBack)
            {
                ShowWorking(StockToCancelViewModel);
                Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(EOrderState.ORDER, objDocument))
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (objDocument != null) objDocument.Response = task.Result.Content;
                            StockToCancelViewModel.PutBackCommand.Execute(objDocument);
                            HideWorking(StockToCancelViewModel);
                        }
                        catch (Exception objException)
                        {
                            CErrorHandling.Log( objException, true);
                            HideWorking(StockToCancelViewModel);
                        }
                    });
            }
            else if (strSelectedAction == StockToCancelViewModel.LabelShow)
            {
                StockToCancelViewModel.ShowCommand.Execute(objDocument);
                HideWorking(StockToCancelViewModel);
            }

            return base.OnContextItemSelected(objItem);
        }
    }
}