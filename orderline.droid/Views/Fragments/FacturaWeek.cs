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
    public sealed class FacturaWeek : BaseTabWidgetFragment
    {
        public FacturaWeekViewModel FacturaWeekViewModel => (ViewModel as FacturaWeekViewModel);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _objInflator = inflater;

            var objView = this.BindingInflate(orderline.droid.Resource.Layout.ImportToDeleteFragment, null);

            if (SingletonService.FacturaWeekViewModel == null)
                SingletonService.FacturaWeekViewModel = CMvvmCrossTools.LoadViewModel<FacturaWeekViewModel>();

            ViewModel = SingletonService.FacturaWeekViewModel;
            EnableDaysButton = true;
            ShowDaysButton = true;
            EnablePrintButton = true;
            ShowPrintButton = true;
            RefreshState = EOrderState.FACTURAPRINTED;
            FacturaWeekViewModel.OnRemoteDocumentChanged += RefreshOrders;

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.stocktoimport_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(FacturaWeekViewModel.LabelPrintFactura);
            objMenu.Add(FacturaWeekViewModel.LabelShow);
        }

        protected override bool OnDaysClicked(IMenuItem objMenuItem)
        {
            var dayOfWeek =  DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)DateTime.Now.DayOfWeek;
            var weekNumber = (DateTime.Now.DayOfYear - dayOfWeek + 10) / 7;

            var dialog = CTools.CreateDialog(
                Activity, _objInflator,
                "Woche",
                $"{weekNumber}",
                (sender, args) => { RefreshWithDate(sender as AlertDialog, args); },
                orderline.droid.Resource.Layout.InputDigit,
                orderline.droid.Resource.Id.inputdigit_edittext);

            dialog.Show();
            return true;
        }


        private void RefreshWithDate(AlertDialog sender, DialogClickEventArgs args)
        {
            if (sender == null) return;
            var objInputText = sender.FindViewById<TextView>(orderline.droid.Resource.Id.inputdigit_edittext);
            if (objInputText != null)
            {
                var textDate = objInputText.Text;
                int week = int.Parse(textDate);
                int year = DateTime.Now.Year;

                var begin = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
                var end = begin.AddDays(6);

                RefreshOrders(RefreshState, FacturaWeekViewModel, begin, end);
            }
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objOrder = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Order;
            var settingService = (CSettingService)Mvx.IoCProvider.Resolve<ISettingService>();

            ShowWorking(FacturaWeekViewModel);

            if (strSelectedAction == FacturaWeekViewModel.LabelShow)
            {
                FacturaWeekViewModel.ShowCommand.Execute(objOrder);
            }
            else if (strSelectedAction == FacturaWeekViewModel.LabelPrintFactura)
            {
                objOrder.Response = settingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                FacturaWeekViewModel.PrintOrderCommand.ExecuteAsync(objOrder);
            }

            HideWorking(FacturaWeekViewModel);
            return base.OnContextItemSelected(objItem);
        }
    }
}