using System;
using System.Collections.Generic;
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
    public sealed class FacturaAdress : BaseTabWidgetFragment
    {
        public FacturaAdressViewModel FacturaAdressViewModel => (ViewModel as FacturaAdressViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _objInflator = inflater;

            var objView = this.BindingInflate(orderline.droid.Resource.Layout.ImportToDeleteFragment, null);

            if (SingletonService.FacturaAllViewModel == null)
                SingletonService.FacturaAllViewModel = CMvvmCrossTools.LoadViewModel<FacturaAdressViewModel>();

            ViewModel = SingletonService.FacturaAllViewModel;
            EnablePrintButton = true;
            ShowPrintButton = true;
            ShowDaysButton = true;
            EnableDaysButton = true;
            RefreshState = EOrderState.FACTURAPRINTED;
            FacturaAdressViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(FacturaAdressViewModel.LabelPrintFactura);
            objMenu.Add(FacturaAdressViewModel.LabelShow);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            var dialog = CTools.CreateDialog(
                Activity, _objInflator,
                "KONTO", "",
                (sender, args) => { RefreshWithAdressNumber(sender as AlertDialog, args); },
                orderline.droid.Resource.Layout.InputDigit,
                orderline.droid.Resource.Id.inputdigit_edittext);

            dialog.Show();
            return true;
        }

        private void RefreshWithAdressNumber(AlertDialog sender, DialogClickEventArgs args)
        {
            if (sender == null) return;
            var objInputText = sender.FindViewById<TextView>(orderline.droid.Resource.Id.inputdigit_edittext);
            if (objInputText != null)
            {
                var adressNumber = objInputText.Text;
                RefreshOrders(RefreshState, FacturaAdressViewModel, null, null, adressNumber);
            }
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objOrder = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;
            var settingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();

            ShowWorking(FacturaAdressViewModel);

            if (strSelectedAction == FacturaAdressViewModel.LabelShow)
            {
                FacturaAdressViewModel.ShowCommand.Execute(objOrder);
            }
            else if (strSelectedAction == FacturaAdressViewModel.LabelPrintFactura)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                FacturaAdressViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }

            HideWorking(FacturaAdressViewModel);
            return base.OnContextItemSelected(objItem);
        }
    }
}