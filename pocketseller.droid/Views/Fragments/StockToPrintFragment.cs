using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class StockToPrintFragment : BaseTabWidgetFragment
    {
        public StockToPrintViewModel StockToPrintViewModel => (ViewModel as StockToPrintViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.StockToPrintFragment, null);

            if (SingletonService.StockToPrintViewModel == null)
                SingletonService.StockToPrintViewModel = CMvvmCrossTools.LoadViewModel<StockToPrintViewModel>();

            ViewModel = SingletonService.StockToPrintViewModel;
            EnablePrintButton = false;
            StockToPrintViewModel.OnRemoteDocumentChanged += RefreshOrders;
            RefreshState = EOrderState.ORDER;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoprint_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View objView, IContextMenuContextMenuInfo objMenuInfo)
        {
            base.OnCreateContextMenu(objMenu, objView, objMenuInfo);
            objMenu.Add(StockToPrintViewModel.LabelPrintComission);
            objMenu.Add(StockToPrintViewModel.LabelEditComission);
            objMenu.Add(StockToPrintViewModel.LabelPrintDeliveryNoteWithPrice);
            objMenu.Add(StockToPrintViewModel.LabelPrintDeliveryNoteWithoutPrice);
            objMenu.Add(StockToPrintViewModel.LabelReady);
            objMenu.Add(StockToPrintViewModel.LabelShow);
            //objMenu.Add(StockToPrintViewModel.LabelCancel);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            return true;
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objOrder = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;

            var settingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();

            ShowWorking(StockToPrintViewModel);
            
            if (strSelectedAction == StockToPrintViewModel.LabelPrintComission)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetComissionTemplate);
                StockToPrintViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelEditComission)
            {
                StockToPrintViewModel.EditOrderCommand.Execute(objOrder);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelPrintDeliveryNoteWithPrice)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetDeliveryWithPriceTemplate);
                StockToPrintViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelPrintDeliveryNoteWithoutPrice)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetDeliveryWithoutPriceTemplate);
                StockToPrintViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelShow)
            {
                StockToPrintViewModel.ShowCommand.Execute(objOrder);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelReady)
            {
                ChangeOrderState(EOrderState.DELIVERY, objOrder);
                RefreshOrders(EOrderState.ORDER, this.StockToPrintViewModel);
            }
            else if (strSelectedAction == StockToPrintViewModel.LabelCancel)
            {
                ChangeOrderState(EOrderState.CANCELED, objOrder);
                RefreshOrders(EOrderState.ORDER, this.StockToPrintViewModel);
            }

            HideWorking(StockToPrintViewModel);

            return base.OnContextItemSelected(objItem);
        }

        private void ChangeOrderState(EOrderState toState, Order objOrder)
        {
            ShowWorking(StockToPrintViewModel);
            Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(toState, objOrder))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (objOrder != null) objOrder.Response = task.Result.Content;
                        StockToPrintViewModel.CancelCommand.Execute(objOrder);
                        HideWorking(StockToPrintViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(StockToPrintViewModel);
                    }
                });
        }
    }
}