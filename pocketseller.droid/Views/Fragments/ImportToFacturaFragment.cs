using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using MvvmCross;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class ImportToFacturaFragment : BaseTabWidgetFragment
    {
        public ImportToFacturaViewModel ImportToFacturaViewModel => (ViewModel as ImportToFacturaViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.ImportToFacturaFragment, null);

            if (SingletonService.ImportToFacturaViewModel == null)
                SingletonService.ImportToFacturaViewModel = CMvvmCrossTools.LoadViewModel<ImportToFacturaViewModel>();

            ViewModel = SingletonService.ImportToFacturaViewModel;
            EnablePrintButton = false;
            RefreshState = EOrderState.FACTURA;
            ImportToFacturaViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktodelete_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(ImportToFacturaViewModel.LabelImportToFactura);
            objMenu.Add(ImportToFacturaViewModel.LabelImportAsCreditNote);
            objMenu.Add(ImportToFacturaViewModel.LabelPrintDeliveryNoteWithPrice);
            objMenu.Add(ImportToFacturaViewModel.LabelPrintDeliveryNoteWithoutPrice);
            objMenu.Add(ImportToFacturaViewModel.LabelShow);
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

            ShowWorking(ImportToFacturaViewModel);
            
            if (strSelectedAction == ImportToFacturaViewModel.LabelImportToFactura)
            {
                if (objOrder.TotalBrutto < 0 && objOrder.DocumentType == (int)EDocumentType.CREDITNOTE)
                {
                    CTools.ShowMessage("Fehler", $"Beleg ist eine Gutschrift mit Betrag {objOrder.TotalBrutto}");
                }
                else
                {
                    CTools.ShowDialog(Activity
                        , Language.Attention
                        , Language.DoYouWantImport
                        , Language.Yes
                        , (sender, args) => ImportToErp(objOrder, ETargetDocumentType.Factura)
                        , Language.No
                        , (sender, args) => { }
                        , string.Empty
                        , null);
                }
            }
            else if (strSelectedAction == ImportToFacturaViewModel.LabelImportAsCreditNote)
            {
                if (objOrder.TotalBrutto > 0 && objOrder.DocumentType != (int)EDocumentType.CREDITNOTE)
                {
                    CTools.ShowMessage("Fehler", $"Beleg ist eine Bestellung mit Betrag {objOrder.TotalBrutto}");
                }
                else
                {
                    CTools.ShowDialog(Activity
                        , Language.Attention
                        , Language.DoYouWantImport
                        , Language.Yes
                        , (sender, args) => ImportToErp(objOrder, ETargetDocumentType.CreditNote)
                        , Language.No
                        , (sender, args) => { }
                        , string.Empty
                        , null);
                }
            }
            else if (strSelectedAction == ImportToFacturaViewModel.LabelPrintDeliveryNoteWithPrice)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetDeliveryWithPriceTemplate);
                ImportToFacturaViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == ImportToFacturaViewModel.LabelPrintDeliveryNoteWithoutPrice)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetDeliveryWithoutPriceTemplate);
                ImportToFacturaViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == ImportToFacturaViewModel.LabelShow)
            {
                ImportToFacturaViewModel.ShowCommand.Execute(objOrder);
            }

            HideWorking(ImportToFacturaViewModel);

            return base.OnContextItemSelected(objItem);
        }

        private void ImportToErp(Order objOrder, ETargetDocumentType targetType)
        {
            ShowWorking(ImportToFacturaViewModel);
            Task.Run(() => CGmWebServices.Instance.ImportToErp(objOrder, targetType))
                .ContinueWith(objImportTask =>
                {
                    try
                    {
                        if (objOrder != null) objOrder.Response = objImportTask.Result.Content;
                        ImportToFacturaViewModel.ImportCommand.Execute(objOrder);
                        HideWorking(ImportToFacturaViewModel);
                        RefreshOrders(EOrderState.FACTURA, this.ImportToFacturaViewModel);
                    }
                    catch (Exception objException)
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(ImportToFacturaViewModel);
                    }
                });
        }
    }
}