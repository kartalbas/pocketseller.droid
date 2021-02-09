using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using pocketseller.core.Models;
using orderline.core.Resources;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using RestSharp;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DataInterfaceFragment : MvxFragment
    {
        public DataInterfaceViewModel DataInterfaceViewModel => (ViewModel as DataInterfaceViewModel);

        private CDataService _objDataService;
        private string LogTag;
        private int _iStatusClickCount;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objThisView = this.BindingInflate(orderline.droid.Resource.Layout.DataInterfaceView, null);

            LogTag = GetType().ToString();
            _objDataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();

            var objButtonAll = objThisView.FindViewById<Button>(orderline.droid.Resource.Id.datainterface_button_renewall);
            objButtonAll.Click += ButtonAllOnClick;

            var objStatus = objThisView.FindViewById<TextView>(orderline.droid.Resource.Id.datainterface_status);
            objStatus.Click += (sender, args) =>
            {
                _iStatusClickCount++;
                if (_iStatusClickCount == 5)
                {
                    _objDataService.RecreatePocketsellerTables();
                    _iStatusClickCount = 0;
                    Activity.Finish();
                }
            };

            return objThisView;
        }

        private async void ButtonAllOnClick(object sender, EventArgs eventArgs)
        {
            if (await DownloadAndImportCompany())
                if (await DownloadAndImportAddress())
                    if (await DownloadAndImportArticle())
                        if (await DownloadAndImportArticlePrice())
                            if (await DownloadAndImportOutstandingpayments())
                                await DownloadAndImportLastprice();
        }

        private async Task<bool> DownloadAndImportCompany()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await CGmWebServices.Instance.DownloadAndImport<List<Company>, List<Company>, Company>(ESettingType.RestCompany, null, DataInterfaceViewModel)
                .ContinueWith(delegate (Task<IRestResponse> objTask)
                {
                    try
                    {
                        if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            objThisTask.SetResult(true);
                        }
                        else
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            objThisTask.SetResult(false);
                            if (objTask != null)
                                CErrorHandling.Log(Language.Attention, "Company Failed: " + objTask.Result.Content, true);
                        }
                    }
                    catch (Exception objException)
                    {
                        DataInterfaceViewModel.ControlIsEnabled = true;
                        CErrorHandling.Log(objException, true);
                        objThisTask.SetResult(false);
                    }
                });

            return objThisTask.Task.Result;
        }

        private async Task<bool> DownloadAndImportAddress()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await CGmWebServices.Instance.DownloadAndImport<List<Adress>, List<Adress>, Adress>(ESettingType.RestAddress, null, DataInterfaceViewModel)
                .ContinueWith(delegate (Task<IRestResponse> objTask)
                {
                    try
                    {
                        if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            DataInterfaceViewModel.DurationAddress = DateTime.Now.Subtract(dStart).TotalSeconds;
                            objThisTask.SetResult(true);
                        }
                        else
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            objThisTask.SetResult(false);
                            if (objTask != null)
                                CErrorHandling.Log(Language.Attention, "Address Failed: " + objTask.Result.Content, true);
                        }
                    }
                    catch (Exception objException)
                    {
                        DataInterfaceViewModel.ControlIsEnabled = true;
                        CErrorHandling.Log(objException, true);
                        objThisTask.SetResult(false);
                    }
                });

            return objThisTask.Task.Result;
        }

        private async Task<bool> DownloadAndImportLastprice()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await
                CGmWebServices.Instance.DownloadAndImport<List<Lastprice>, List<Lastprice>, Lastprice>(ESettingType.RestLastprices, null, DataInterfaceViewModel)
                    .ContinueWith(delegate (Task<IRestResponse> objTask)
                    {
                        try
                        {
                            if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                            {
                                DataInterfaceViewModel.DurationLastprice = DateTime.Now.Subtract(dStart).TotalSeconds;
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(true);
                            }
                            else
                            {
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(false);
                                if (objTask != null)
                                    CErrorHandling.Log(Language.Attention,
                                        "Lastprice Failed: " + objTask.Result.Content, true);
                            }
                        }
                        catch (Exception objException)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            CErrorHandling.Log(objException, true);
                            objThisTask.SetResult(false);
                        }
                    });

            return objThisTask.Task.Result;
        }

        private async Task<bool> DownloadAndImportArticle()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await
                CGmWebServices.Instance.DownloadAndImport<List<Article>, List<Article>, Article>(ESettingType.RestArticle, null, DataInterfaceViewModel)
                    .ContinueWith(delegate (Task<IRestResponse> objTask)
                    {
                        try
                        {
                            if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                            {
                                DataInterfaceViewModel.DurationArticle = DateTime.Now.Subtract(dStart).TotalSeconds;

                                dStart = DateTime.Now;

                                DataInterfaceViewModel.OnStatusUpdate(
                                    typeof(Articleprice).Name + " " +
                                    DataInterfaceViewModel.EState.Converting, EventArgs.Empty);

                                //var cobjArticles = _objDataService.PocketsellerConnection.Table<Article>().ToList();

                                DataInterfaceViewModel.OnStatusUpdate(
                                    typeof(Articleprice).Name + " " +
                                    DataInterfaceViewModel.EState.Importing, EventArgs.Empty);

                                DataInterfaceViewModel.DurationArticleprice = DateTime.Now.Subtract(dStart).TotalSeconds;
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(true);
                            }
                            else
                            {
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(false);
                                if (objTask != null)
                                    CErrorHandling.Log(Language.Attention,
                                        "Article Failed: " + objTask.Result.Content, true);
                            }
                        }
                        catch (Exception objException)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            CErrorHandling.Log(objException, true);
                            objThisTask.SetResult(false);
                        }
                    });

            return objThisTask.Task.Result;
        }

        private async Task<bool> DownloadAndImportArticlePrice()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await
                CGmWebServices.Instance.DownloadAndImport<List<Articleprice>, List<Articleprice>, Articleprice>(ESettingType.RestArticlePrice, null, DataInterfaceViewModel)
                    .ContinueWith(delegate (Task<IRestResponse> objTask)
                    {
                        try
                        {
                            if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                            {
                                DataInterfaceViewModel.DurationArticleprice = DateTime.Now.Subtract(dStart).TotalSeconds;
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(true);
                            }
                            else
                            {
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(false);
                                if (objTask != null)
                                    CErrorHandling.Log(Language.Attention,
                                        "Pricegroup Failed: " + objTask.Result.Content, true);
                            }
                        }
                        catch (Exception objException)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            CErrorHandling.Log(objException, true);
                            objThisTask.SetResult(false);
                        }
                    });

            return objThisTask.Task.Result;
        }

        private async Task<bool> DownloadAndImportOutstandingpayments()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;
            var objThisTask = new TaskCompletionSource<bool>();

            await
                CGmWebServices.Instance.DownloadAndImport<List<OpenPayment>, List<OpenPayment>, OpenPayment>(ESettingType.RestOutstandingpayments, null, DataInterfaceViewModel)
                    .ContinueWith(delegate (Task<IRestResponse> objTask)
                    {
                        try
                        {
                            if (objTask != null && objTask.Result.Content.Replace("\"", "") == Globals.TRUE)
                            {
                                DataInterfaceViewModel.DurationOutstandingpayments = DateTime.Now.Subtract(dStart).TotalSeconds;
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(true);
                            }
                            else
                            {
                                DataInterfaceViewModel.ControlIsEnabled = true;
                                objThisTask.SetResult(false);
                                if (objTask != null)
                                    CErrorHandling.Log(Language.Attention,
                                        "Outstandingpayments Failed: " + objTask.Result.Content, true);
                            }
                        }
                        catch (Exception objException)
                        {
                            DataInterfaceViewModel.ControlIsEnabled = true;
                            CErrorHandling.Log(objException, true);
                            objThisTask.SetResult(false);
                        }
                    });

            return objThisTask.Task.Result;
        }
    }
}