using System;
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
    public sealed class FacturaDefect : BaseTabWidgetFragment
    {
        public FacturaDefectViewModel FacturaDefectViewModel => (ViewModel as FacturaDefectViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _objInflator = inflater;

            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.ImportToDeleteFragment, null);

            if (SingletonService.FacturaDefectViewModel == null)
                SingletonService.FacturaDefectViewModel = CMvvmCrossTools.LoadViewModel<FacturaDefectViewModel>();

            ViewModel = SingletonService.FacturaDefectViewModel;
            EnablePrintButton = true;
            ShowPrintButton = true;
            EnableDaysButton = true;
            ShowDaysButton = true;
            RefreshDefects = true;
            RefreshState = EOrderState.FACTURAPRINTED;
            FacturaDefectViewModel.OnRemoteDocumentChanged += RefreshOrdersDefects;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(FacturaDefectViewModel.LabelPrintFactura);
            objMenu.Add(FacturaDefectViewModel.LabelShow);
            objMenu.Add(FacturaDefectViewModel.LabelCorrectFactura);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            var dialog = CTools.CreateDialog(
                Activity, _objInflator,
                "KONTO", "",
                (sender, args) => { RefreshWithAdressNumber(sender as AlertDialog, args); },
                pocketseller.droid.Resource.Layout.InputDigit,
                pocketseller.droid.Resource.Id.inputdigit_edittext);

            dialog.Show();
            return true;
        }

        private void RefreshWithAdressNumber(AlertDialog sender, DialogClickEventArgs args)
        {
            if (sender == null) return;
            var objInputText = sender.FindViewById<TextView>(pocketseller.droid.Resource.Id.inputdigit_edittext);
            if (objInputText != null)
            {
                var adressNumber = objInputText.Text;
                RefreshOrdersDefectsByAdress(RefreshState, FacturaDefectViewModel, null, null, adressNumber);
            }
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objOrder = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;
            var settingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();

            ShowWorking(FacturaDefectViewModel);

            if (strSelectedAction == FacturaDefectViewModel.LabelShow)
            {
                FacturaDefectViewModel.ShowCommand.Execute(objOrder);
            }
            else if (strSelectedAction == FacturaDefectViewModel.LabelPrintFactura)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                FacturaDefectViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }
            else if (strSelectedAction == FacturaDefectViewModel.LabelCorrectFactura)
            {
            }

            HideWorking(FacturaDefectViewModel);
            return base.OnContextItemSelected(objItem);
        }
    }
}