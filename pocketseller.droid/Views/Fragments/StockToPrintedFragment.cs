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
    public sealed class StockToPrintedFragment : BaseTabWidgetFragment
    {
        public StockToPrintedViewModel StockToPrintedViewModel => (ViewModel as StockToPrintedViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.StockToPrintFragment, null);

            if (SingletonService.StockToPrintedViewModel == null)
                SingletonService.StockToPrintedViewModel = CMvvmCrossTools.LoadViewModel<StockToPrintedViewModel>();

            ViewModel = SingletonService.StockToPrintedViewModel;
            EnablePrintButton = false;
            RefreshState = EOrderState.DELIVERY;
            StockToPrintedViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoprint_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View objView, IContextMenuContextMenuInfo objMenuInfo)
        {
            base.OnCreateContextMenu(objMenu, objView, objMenuInfo);
            objMenu.Add(StockToPrintedViewModel.LabelCancel);
            objMenu.Add(StockToPrintedViewModel.LabelShow);
            objMenu.Add(StockToPrintedViewModel.LabelPutBack);
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

            if (strSelectedAction == StockToPrintedViewModel.LabelPutBack)
            {
                ShowWorking(StockToPrintedViewModel);
                Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(EOrderState.ORDER, objDocument))
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (objDocument != null) objDocument.Response = task.Result.Content;
                            StockToPrintedViewModel.PutBackCommand.Execute(objDocument);
                            HideWorking(StockToPrintedViewModel);
                        }
                        catch (Exception objException)
                        {
                            CErrorHandling.Log( objException, true);
                            HideWorking(StockToPrintedViewModel);
                        }
                    });
            }
            else if (strSelectedAction == StockToPrintedViewModel.LabelCancel)
            {
                ShowWorking(StockToPrintedViewModel);
                Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(EOrderState.CANCELED, objDocument))
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (objDocument != null) objDocument.Response = task.Result.Content;
                            StockToPrintedViewModel.CancelCommand.Execute(objDocument);
                            HideWorking(StockToPrintedViewModel);
                        }
                        catch (Exception objException)
                        {
                            CErrorHandling.Log( objException, true);
                            HideWorking(StockToPrintedViewModel);
                        }
                    });
            }
            else if (strSelectedAction == StockToPrintedViewModel.LabelShow)
            {
                StockToPrintedViewModel.ShowCommand.Execute(objDocument);
                HideWorking(StockToPrintedViewModel);
            }

            return base.OnContextItemSelected(objItem);
        }
    }
}