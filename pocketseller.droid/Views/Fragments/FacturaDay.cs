using System;
using System.Globalization;
using Android.App;
using Android.Content;
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
    public sealed class FacturaDay : BaseTabWidgetFragment
    {
        public FacturaDayViewModel FacturaDaysViewModel => (ViewModel as FacturaDayViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _objInflator = inflater;

            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.ImportToDeleteFragment, null);

            if (SingletonService.FacturaDayViewModel == null)
                SingletonService.FacturaDayViewModel = CMvvmCrossTools.LoadViewModel<FacturaDayViewModel>();

            ViewModel = SingletonService.FacturaDayViewModel;
            EnableDaysButton = true;
            ShowDaysButton = true;
            EnablePrintButton = true;
            ShowPrintButton = true;
            RefreshState = EOrderState.FACTURAPRINTED;
            FacturaDaysViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(FacturaDaysViewModel.LabelPrintFactura);
            objMenu.Add(FacturaDaysViewModel.LabelShow);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            var dialog = CTools.CreateDialog(
                Activity, _objInflator,
                "Datum", $"{DateTime.Now.Day}",
                (sender, args) => { RefreshWithDate(sender as AlertDialog, args); },
                pocketseller.droid.Resource.Layout.InputDate,
                pocketseller.droid.Resource.Id.inputdate_edittext);

            dialog.Show();
            return true;
        }

        private void RefreshWithDate(AlertDialog sender, DialogClickEventArgs args)
        {
            if (sender == null) return;
            var objInputText = sender.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdate_edittext);
            if (objInputText != null)
            {
                var textDate = objInputText.Text;
                if (textDate.Length < 3)
                {
                    textDate = textDate + "." + DateTime.Now.Month;
                }
                if (textDate.Length < 6)
                {
                    textDate = textDate + "." + DateTime.Now.Year;
                }

                var date = DateTime.Parse(textDate, new CultureInfo("de-DE"));
                RefreshOrders(RefreshState, FacturaDaysViewModel, date, date);
            }
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objOrder = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;
            var settingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();

            ShowWorking(FacturaDaysViewModel);

            if (strSelectedAction == FacturaDaysViewModel.LabelShow)
            {
                FacturaDaysViewModel.ShowCommand.Execute(objOrder);
            }
            else if (strSelectedAction == FacturaDaysViewModel.LabelPrintFactura)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                FacturaDaysViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }

            HideWorking(FacturaDaysViewModel);
            return base.OnContextItemSelected(objItem);
        }
    }
}