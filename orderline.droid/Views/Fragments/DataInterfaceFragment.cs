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

            var objUpdateAll = objThisView.FindViewById<Button>(orderline.droid.Resource.Id.datainterface_button_update);
            objUpdateAll.Click += ButtonUpdateOnClick;

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

        private async void ButtonUpdateOnClick(object sender, EventArgs e)
        {
            try
            {
                DataInterfaceViewModel.ControlIsEnabled = false;

                var restService = Mvx.IoCProvider.Resolve<IRestService>();
                var stocks = await restService.GetAllStocks();

                var dataService = (CDataService)Mvx.IoCProvider.Resolve<IDataService>();

                foreach (var stock in stocks)
                {
                    var article = Article.FindByArticleNr(stock.Articlenumber);
                    if(article != null)
                    {
                        article.Content = stock.Content;
                        article.StockName = stock.StockName;
                        article.StockPlace = stock.StockPlace;
                        article.StockAmount = stock.StockAmount;
                        dataService.PocketsellerConnection.Update(article);
                    }
                }

                DataInterfaceViewModel.ControlIsEnabled = true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
            }
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
            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<Company>, List<Company>, Company>(ESettingType.RestCompany, null, DataInterfaceViewModel);

                DataInterfaceViewModel.ControlIsEnabled = true;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    CErrorHandling.Log(Language.Attention, "Company Failed: " + result.Content, true);
                    return false;
                }

                return true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }

        }

        private async Task<bool> DownloadAndImportAddress()
        {
            var dStart = DateTime.Now;
            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<Adress>, List<Adress>, Adress>(ESettingType.RestAddress, null, DataInterfaceViewModel);

                DataInterfaceViewModel.ControlIsEnabled = true;
                DataInterfaceViewModel.DurationAddress = DateTime.Now.Subtract(dStart).TotalSeconds;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    CErrorHandling.Log(Language.Attention, "Address Failed: " + result.Content, true);
                    return false;
                }

                return true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }
        }

        private async Task<bool> DownloadAndImportLastprice()
        {
            var dStart = DateTime.Now;
            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<Lastprice>, List<Lastprice>, Lastprice>(ESettingType.RestLastprices, null, DataInterfaceViewModel);

                DataInterfaceViewModel.ControlIsEnabled = true;
                DataInterfaceViewModel.DurationLastprice = DateTime.Now.Subtract(dStart).TotalSeconds;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    CErrorHandling.Log(Language.Attention, "Lastprice Failed: " + result.Content, true);
                    return false;
                }

                return true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }
        }

        private async Task<bool> DownloadAndImportArticle()
        {
            var dStart = DateTime.Now;
            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<Article>, List<Article>, Article>(ESettingType.RestArticle, null, DataInterfaceViewModel);
                DataInterfaceViewModel.DurationArticle = DateTime.Now.Subtract(dStart).TotalSeconds;
                DataInterfaceViewModel.ControlIsEnabled = true;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    DataInterfaceViewModel.ControlIsEnabled = true;
                    CErrorHandling.Log(Language.Attention, "Article Failed: " + result.Content, true);
                    return false;
                }

                return true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }
        }

        private async Task<bool> DownloadAndImportArticlePrice()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<Articleprice>, List<Articleprice>, Articleprice>(ESettingType.RestArticlePrice, null, DataInterfaceViewModel);
                DataInterfaceViewModel.DurationArticleprice = DateTime.Now.Subtract(dStart).TotalSeconds;
                DataInterfaceViewModel.ControlIsEnabled = true;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    DataInterfaceViewModel.ControlIsEnabled = true;
                    CErrorHandling.Log(Language.Attention, "Pricegroup Failed: " + result.Content, true);
                }
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }

            return true;
        }

        private async Task<bool> DownloadAndImportOutstandingpayments()
        {
            var dStart = DateTime.Now;

            DataInterfaceViewModel.ControlIsEnabled = false;

            try
            {
                var result = await CGmWebServices.Instance.DownloadAndImport<List<OpenPayment>, List<OpenPayment>, OpenPayment>(ESettingType.RestOutstandingpayments, null, DataInterfaceViewModel);

                DataInterfaceViewModel.DurationOutstandingpayments = DateTime.Now.Subtract(dStart).TotalSeconds;
                DataInterfaceViewModel.ControlIsEnabled = true;

                if (result.Content.Replace("\"", "") != Globals.TRUE)
                {
                    DataInterfaceViewModel.ControlIsEnabled = true;
                    CErrorHandling.Log(Language.Attention, "Outstandingpayments Failed: " + result.Content, true);
                    return false;
                }

                return true;
            }
            catch (Exception objException)
            {
                DataInterfaceViewModel.ControlIsEnabled = true;
                CErrorHandling.Log(objException, true);
                return false;
            }
        }
    }
}