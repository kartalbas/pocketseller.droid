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
    public sealed class ImportToDeleteFragment : BaseTabWidgetFragment
    {
        public ImportToDeleteViewModel ImportToDeleteViewModel => (ViewModel as ImportToDeleteViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.ImportToDeleteFragment, null);

            if (SingletonService.ImportToDeleteViewModel== null)
                SingletonService.ImportToDeleteViewModel = CMvvmCrossTools.LoadViewModel<ImportToDeleteViewModel>();

            ViewModel = SingletonService.ImportToDeleteViewModel;
            EnablePrintButton = false;
            RefreshState = EOrderState.FACTURAIMPORTED;
            ImportToDeleteViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(ImportToDeleteViewModel.LabelPrintFactura);
            objMenu.Add(ImportToDeleteViewModel.LabelPrintCash);
            objMenu.Add(ImportToDeleteViewModel.LabelReady);
            objMenu.Add(ImportToDeleteViewModel.LabelShow);
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

            ShowWorking(ImportToDeleteViewModel);
            
            if (strSelectedAction == ImportToDeleteViewModel.LabelShow)
            {
                ImportToDeleteViewModel.ShowCommand.Execute(objOrder);
                HideWorking(ImportToDeleteViewModel);
            }
            else if (strSelectedAction == ImportToDeleteViewModel.LabelPrintFactura)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                ImportToDeleteViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == ImportToDeleteViewModel.LabelPrintCash)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetCashTemplate);
                ImportToDeleteViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == ImportToDeleteViewModel.LabelReady)
            {
                ChangeOrderState(EOrderState.FACTURAPRINTED, objOrder);
            }

            HideWorking(ImportToDeleteViewModel);

            return base.OnContextItemSelected(objItem);
        }

        private void ChangeOrderState(EOrderState toState, Order objOrder)
        {
            ShowWorking(ImportToDeleteViewModel);
            Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(toState, objOrder))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (objOrder != null) objOrder.Response = task.Result.Content;
                        ImportToDeleteViewModel.ReadyCommand.Execute(objOrder);
                        HideWorking(ImportToDeleteViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(ImportToDeleteViewModel);
                    }
                });
        }

    }
}