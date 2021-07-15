using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class ImportToDeliveryFragment : BaseTabWidgetFragment
    {
        public ImportToDeliveryViewModel ImportToDeliveryViewModel => (ViewModel as ImportToDeliveryViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.ImportToImportFragment, null);

            if (SingletonService.ImportToDeliveryViewModel == null)
                SingletonService.ImportToDeliveryViewModel = CMvvmCrossTools.LoadViewModel<ImportToDeliveryViewModel>();

            ViewModel = SingletonService.ImportToDeliveryViewModel;
            EnablePrintButton = false;
            RefreshState = EOrderState.DELIVERY;
            ImportToDeliveryViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(ImportToDeliveryViewModel.LabelErpExport);
            objMenu.Add(ImportToDeliveryViewModel.LabelImportToDelivery);
            objMenu.Add(ImportToDeliveryViewModel.LabelShow);
            objMenu.Add(ImportToDeliveryViewModel.LabelPutBack);
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

            if (strSelectedAction == ImportToDeliveryViewModel.LabelImportToDelivery)
            {
                CTools.ShowDialog(Activity
                    , Language.Attention
                    , Language.DoYouWantImport
                    , Language.Yes
                    , (sender, args) => ImportToErpAsDelivery(objDocument)
                    , Language.No
                    , (sender, args) => { }
                    , string.Empty
                    , null);
            }
            else if (strSelectedAction == ImportToDeliveryViewModel.LabelErpExport)
            {
                CTools.ShowDialog(Activity
                    , Language.Attention
                    , Language.DoYouWantImport
                    , Language.Yes
                    , (sender, args) => ExportToErp(objDocument)
                    , Language.No
                    , (sender, args) => { }
                    , string.Empty
                    , null);
            }
            else if (strSelectedAction == ImportToDeliveryViewModel.LabelPutBack)
            {
                ShowWorking(ImportToDeliveryViewModel);
                Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(EOrderState.ORDER, objDocument))
                    .ContinueWith(task =>
                    {
                        try
                        {
                            if (objDocument != null) objDocument.Response = task.Result.Content;
                            ImportToDeliveryViewModel.PutBackCommand.Execute(objDocument);
                        }
                        catch (Exception objException)
                        {
                            CErrorHandling.Log(objException, true);
                            HideWorking(ImportToDeliveryViewModel);
                        }
                    });
            }
            else if (strSelectedAction == ImportToDeliveryViewModel.LabelDelete)
            {
                CTools.ShowDialog(Activity
                    , Language.Attention
                    , Language.DoYouWantDelete
                    , Language.Yes
                    , (sender, args) => DeleteOrder(objDocument)
                    , Language.No
                    , (sender, args) => { }
                    , string.Empty
                    , null);
            }
            else if (strSelectedAction == ImportToDeliveryViewModel.LabelShow)
            {
                ImportToDeliveryViewModel.ShowCommand.Execute(objDocument);
                HideWorking(ImportToDeliveryViewModel);
            }

            return base.OnContextItemSelected(objItem);
        }

        private void DeleteOrder(Order objDocument)
        {
            ShowWorking(ImportToDeliveryViewModel);
            Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(EOrderState.DELETED, objDocument))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (objDocument != null) objDocument.Response = task.Result.Content;
                        ImportToDeliveryViewModel.DeleteCommand.Execute(objDocument);
                        HideWorking(ImportToDeliveryViewModel);
                        RefreshOrders(EOrderState.DELIVERY, this.ImportToDeliveryViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log( objException, true);
                        HideWorking(ImportToDeliveryViewModel);
                    }
                });
        }

        private void ExportToErp(Order objDocument)
        {
            ShowWorking(ImportToDeliveryViewModel);
            Task.Run(() => CGmWebServices.Instance.ExportToErp(objDocument))
                .ContinueWith(objImportTask =>
                {
                    try
                    {
                        if (objDocument != null) objDocument.Response = objImportTask.Result.Content;
                        ImportToDeliveryViewModel.ImportCommand.Execute(objDocument);
                        HideWorking(ImportToDeliveryViewModel);
                        RefreshOrders(EOrderState.DELIVERY, this.ImportToDeliveryViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(ImportToDeliveryViewModel);
                    }
                });
        }

        private void ImportToErpAsDelivery(Order objDocument)
        {
            ShowWorking(ImportToDeliveryViewModel);
            Task.Run(() => CGmWebServices.Instance.ImportToErp(objDocument, ETargetDocumentType.DeliveryNote))
                .ContinueWith(objImportTask =>
                {
                    try
                    {
                        if (objDocument != null) objDocument.Response = objImportTask.Result.Content;
                        ImportToDeliveryViewModel.ImportCommand.Execute(objDocument);
                        HideWorking(ImportToDeliveryViewModel);
                        ChangeOrderState(EOrderState.FACTURA, objDocument);
                        RefreshOrders(EOrderState.DELIVERY, this.ImportToDeliveryViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log( objException, true);
                        HideWorking(ImportToDeliveryViewModel);
                    }
                });
        }

        private void ChangeOrderState(EOrderState toState, Order objOrder)
        {
            ShowWorking(ImportToDeliveryViewModel);
            Task.Run(() => CGmWebServices.Instance.ChangeDocumentState(toState, objOrder))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (objOrder != null) objOrder.Response = task.Result.Content;
                        ImportToDeliveryViewModel.ImportCommand.Execute(objOrder);
                        HideWorking(ImportToDeliveryViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(ImportToDeliveryViewModel);
                    }
                });
        }
    }
}