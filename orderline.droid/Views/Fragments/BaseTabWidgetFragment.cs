using Android.Views;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace pocketseller.droid.Views.Fragments
{
    public abstract class BaseTabWidgetFragment : MvxFragment
    {
        protected MvxListView _objListView;
        protected LayoutInflater _objInflator;
        protected string LogTag { get; set; }
        protected bool ShowMenuItems { get; set; }
        protected bool ShowPrintButton { get; set; }
        protected bool ShowDaysButton { get; set; }
        protected bool EnablePrintButton { get; set; }
        protected bool EnableDaysButton { get; set; }
        protected bool RefreshDefects { get; set; }
        protected CSingletonService SingletonService { get; set; }
        protected EOrderState RefreshState { get; set; }

        protected static readonly object LOCK = new object();

        public BaseTabWidgetFragment()
        {
            SingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();
            LogTag = GetType().ToString();
            ShowMenuItems = true;
            ShowPrintButton = true;
            ShowDaysButton = false;
            EnablePrintButton = false;
            EnableDaysButton = false;
            RefreshDefects = false;
            HasOptionsMenu = true;
        }

        public override void OnResume()
        {
            HideWorking((BaseViewModel)ViewModel);
            base.OnResume();
        }

        public override void OnCreateOptionsMenu(IMenu objMenu, MenuInflater inflater)
        {
            lock (LOCK)
            {
                Activity.RunOnUiThread(() =>
                {
                    objMenu.Clear();

                    if (ShowPrintButton && EnablePrintButton)
                        objMenu.Add("")
                            .SetIcon(orderline.droid.Resource.Drawable.ic_action_upload_dark)
                            .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnPrintClicked))
                            .SetShowAsAction(ShowAsAction.Always);

                    if (ShowDaysButton && EnableDaysButton)
                        objMenu.Add("")
                            .SetIcon(orderline.droid.Resource.Drawable.ic_action_search_dark)
                            .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnDaysClicked))
                            .SetShowAsAction(ShowAsAction.Always);

                    if(RefreshDefects)
                    {
                        if (ShowMenuItems)
                            objMenu.Add("")
                                .SetIcon(orderline.droid.Resource.Drawable.ic_action_refresh_dark)
                                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnRefreshDefectsClicked))
                                .SetShowAsAction(ShowAsAction.Always);
                    }
                    else
                    {
                        if (ShowMenuItems)
                            objMenu.Add("")
                                .SetIcon(orderline.droid.Resource.Drawable.ic_action_refresh_dark)
                                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnRefreshClicked))
                                .SetShowAsAction(ShowAsAction.Always);
                    }


                    base.OnCreateOptionsMenu(objMenu, inflater);
                });
            }
        }

        protected bool OnPrintClicked(IMenuItem objMenuItem)
        {
            var vm = (BaseViewModel)ViewModel;
            ShowWorking(vm);
            try
            {
                if (vm.Orders == null || vm.Orders?.Count() <= 0)
                    return true;

                var orders = new List<Order>();
                foreach (var order in vm.Orders)
                {
                    order.Response = vm.SettingService.Get<string>(ESettingType.RestGetFacturaTemplate);
                    orders.Add(order);
                }

                vm.PrintOrdersCommand.ExecuteAsync(orders);

            }
            finally
            {
                HideWorking(vm);
            }

            return true;
        }

        protected abstract bool OnDaysClicked(IMenuItem objMenuItem);

        protected bool OnRefreshClicked(IMenuItem objMenuItem)
        {
            RefreshOrders(RefreshState, (BaseViewModel)ViewModel);
            return true;
        }

        protected bool OnRefreshDefectsClicked(IMenuItem objMenuItem)
        {
            RefreshOrdersDefects(RefreshState, (BaseViewModel)ViewModel);
            return true;
        }

        protected void RefreshOrders(EOrderState enmState, BaseViewModel objViewModel, DateTime? begin = null, DateTime? end = null, string adressNumber = null)
        {
            lock (LOCK)
            {
                if (begin != null && end != null)
                {
                    Activity.RunOnUiThread(() => CGmWebServices.Instance.RefreshOrders(
                        enmState,
                        begin.Value,
                        end.Value,
                        () => ShowWorking(objViewModel),
                        objOrders =>
                        {
                            objViewModel.Orders = objOrders;
                            HideWorking(objViewModel);
                    },
                    objException =>
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(objViewModel);
                    }));
                }
                else if(!string.IsNullOrEmpty(adressNumber))
                {
                    Activity.RunOnUiThread(() => CGmWebServices.Instance.RefreshOrders(
                        enmState,
                        adressNumber,
                        () => ShowWorking(objViewModel),
                        objOrders =>
                        {
                            objViewModel.Orders = objOrders;
                            HideWorking(objViewModel);
                        },
                    objException =>
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(objViewModel);
                    }));
                }
                else
                {
                    Activity.RunOnUiThread(() => CGmWebServices.Instance.RefreshOrders(enmState,
                    () => ShowWorking(objViewModel),
                    objOrders =>
                    {
                        objViewModel.Orders = objOrders;
                        HideWorking(objViewModel);
                    },
                    objException =>
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(objViewModel);
                    }));
                }
            }
        }

        protected void RefreshOrdersDefectsByAdress(EOrderState enmState, BaseViewModel objViewModel, DateTime? begin = null, DateTime? end = null, string adressNumber = null)
        {
            lock (LOCK)
            {
                if(!string.IsNullOrEmpty(adressNumber))
                {
                    Activity.RunOnUiThread(() => CGmWebServices.Instance.RefreshOrders(enmState, adressNumber,
                        () => ShowWorking(objViewModel),
                        objOrders =>
                        {
                            var issuedOrders = new List<Order>();

                            var taxes = Article.GetTaxes();
                            var tax0 = taxes.Item1;
                            var tax1 = taxes.Item2;

                            foreach (var order in objOrders)
                            {
                                var customer = objViewModel.DataService.FindWithQuery<Adress>($"SELECT * FROM Adress WHERE Adressnumber = '{order.Adressnumber}'");
                                var isOhneMwSt = Adress.IsCustomerWithoutMwSt(customer.Taxgroup);

                                var nettoSums = Orderdetail.GetNettoSum(order.Orderdetails, tax0, tax1);
                                var nettoAmount0 = nettoSums.Item1;
                                var nettoAmount1 = nettoSums.Item2;

                                var bruttoAmount0 = isOhneMwSt ? Math.Round(nettoAmount0, 2, MidpointRounding.AwayFromZero) : Article.GetBruttoPrice(nettoAmount0, tax0);
                                var bruttoAmount1 = isOhneMwSt ? Math.Round(nettoAmount1, 2, MidpointRounding.AwayFromZero) : Article.GetBruttoPrice(nettoAmount1, tax1);

                                var taxAmount0 = isOhneMwSt ? 0 : bruttoAmount0 - nettoAmount0;
                                var taxAmount1 = isOhneMwSt ? 0 : bruttoAmount1 - nettoAmount1;

                                var totalNettoAmount = nettoAmount0 + nettoAmount1;
                                var totalBruttoAmount = bruttoAmount0 + bruttoAmount1;
                                var totalAmount = isOhneMwSt ? totalNettoAmount : totalBruttoAmount;

                                var diff = isOhneMwSt ? totalAmount - order.TotalNetto : totalAmount - order.TotalBrutto;
                                var defectAmount = isOhneMwSt ? order.TotalNetto : order.TotalBrutto;

                                if (diff > 0.02M)
                                {
                                    order.TotalNetto = totalNettoAmount;
                                    order.TotalBrutto = totalAmount;
                                    issuedOrders.Add(order);
                                }
                            }

                            objViewModel.Orders = new ObservableCollection<Order>(issuedOrders);

                            HideWorking(objViewModel);
                        },
                        objException =>
                        {
                            CErrorHandling.Log(objException, true);
                            HideWorking(objViewModel);
                        }));
                }
            }
        }


        protected void RefreshOrdersDefects(EOrderState enmState, BaseViewModel objViewModel, DateTime? begin = null, DateTime? end = null, string adressNumber = null)
        {
            lock (LOCK)
            {
                Activity.RunOnUiThread(() => CGmWebServices.Instance.RefreshOrders(enmState,
                    () => ShowWorking(objViewModel),
                    objOrders =>
                    {
                        if (begin != null && end != null)
                        {
                            begin = new DateTime(begin.Value.Year, begin.Value.Month, begin.Value.Day, 0, 0, 0, 0);
                            end = new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59, 59);
                            var result = objOrders.Where(o => o.TimeStamp >= begin && o.TimeStamp <= end).ToList();
                            objOrders = new ObservableCollection<Order>(result);
                        }

                        var issuedOrders = new List<Order>();

                        var taxes = Article.GetTaxes();
                        var tax0 = taxes.Item1;
                        var tax1 = taxes.Item2;

                        foreach (var order in objOrders)
                        {
                            var customer = objViewModel.DataService.FindWithQuery<Adress>($"SELECT * FROM Adress WHERE Adressnumber = '{order.Adressnumber}'");
                            var isOhneMwSt = Adress.IsCustomerWithoutMwSt(customer.Taxgroup);

                            var nettoSums = Orderdetail.GetNettoSum(order.Orderdetails, tax0, tax1);
                            var nettoAmount0 = nettoSums.Item1;
                            var nettoAmount1 = nettoSums.Item2;

                            var bruttoAmount0 = isOhneMwSt ? Math.Round(nettoAmount0, 2, MidpointRounding.AwayFromZero) : Article.GetBruttoPrice(nettoAmount0, tax0);
                            var bruttoAmount1 = isOhneMwSt ? Math.Round(nettoAmount1, 2, MidpointRounding.AwayFromZero) : Article.GetBruttoPrice(nettoAmount1, tax1);

                            var taxAmount0 = isOhneMwSt ? 0 : bruttoAmount0 - nettoAmount0;
                            var taxAmount1 = isOhneMwSt ? 0 : bruttoAmount1 - nettoAmount1;

                            var totalNettoAmount = nettoAmount0 + nettoAmount1;
                            var totalBruttoAmount = bruttoAmount0 + bruttoAmount1;
                            var totalAmount = isOhneMwSt ? totalNettoAmount : totalBruttoAmount;

                            var diff = isOhneMwSt ? totalAmount - order.TotalNetto : totalAmount - order.TotalBrutto;
                            var defectAmount = isOhneMwSt ? order.TotalNetto : order.TotalBrutto;

                            if (diff > 0.02M)
                            {
                                order.TotalNetto = totalNettoAmount;
                                order.TotalBrutto = totalAmount;
                                issuedOrders.Add(order);
                                //Debug.WriteLine($"@@@{order.TimeStamp.Day}.{order.TimeStamp.Month}.{order.TimeStamp.Year},{order.Adressnumber},{order.Docnumber},{diff.ToString(CultureInfo.CreateSpecificCulture("de-CH"))},{TotalBrutto.ToString(CultureInfo.CreateSpecificCulture("de-CH"))},{(defectAmount).ToString(CultureInfo.CreateSpecificCulture("de-CH"))}");
                            }
                        }

                        objViewModel.Orders = new ObservableCollection<Order>(issuedOrders);

                        HideWorking(objViewModel);
                    },
                    objException =>
                    {
                        CErrorHandling.Log(objException, true);
                        HideWorking(objViewModel);
                    }));
            }
        }

        protected void ShowWorking(BaseViewModel objBaseViewModel)
        {
            lock (LOCK)
            {
                Activity.RunOnUiThread(() =>
                {
                    objBaseViewModel.ShowWorkingCommand.Execute(null);
                    _objListView.Enabled = false;
                    ShowMenuItems = false;
                    ShowPrintButton = false;
                    Activity.InvalidateOptionsMenu();
                });
            }
        }

        protected void HideWorking(BaseViewModel objBaseViewModel)
        {
            lock (LOCK)
            {
                Activity.RunOnUiThread(() =>
                {
                    while (!_objListView.Adapter.AreAllItemsEnabled())
                    {
                    }
                    ShowMenuItems = true;
                    ShowPrintButton = true;
                    Activity.InvalidateOptionsMenu();
                    _objListView.Enabled = true;
                    objBaseViewModel.HideWorkingCommand.Execute(null);
                });
            }
        }
    }
}