using System;
using System.ComponentModel;
using System.Globalization;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.Services
{
    public enum ESettingType
    {
        Init,        
        SearchType,
        SearchTypeAddress,
        SearchTypeArticle,
        KeyboardTypeOnSearch,
        KeyboardTypeInDocumentdetail,
        Language,
        SearchMaxChar,
        DataSourceId,
        RestProtocol,
        RestAddress,
        RestArticle,
        RestStock,
        RestOutstandingpayments,
        RestPayments,
        RestPaymentsdetails,
        RestInvoices,
        RestLastprices,
        RestTax,
        RestArticlePrice,
        RestDocumentAddOrUpdate,
        RestDocumentInsertOrderAsFactura,
        RestDocumentInsertOrderAsDelivery,
        RestDocumentInsertOrderAsCreditNote,
        RestGetDocumentsByState,
        RestChangeDocumentState,
        RestDeleteDocument,
        RestExportToErp,
        RestImportToErpAsDelivery,
        RestImportToErpAsFactura,
        RestGetLatestOrdernumber,
        RestQuotationAddOrUpdate,
        RestGetQuotationsByState,
        RestChangeQuotationState,
        RestDeleteQuotation,
        RestGetLatestQuotationnumber,
        RestGetComissionTemplate,
        RestDataResource,
        RestGetSources,
        RestGetKey,
        RestDataApi,
        RestLoginApi,
        RestSourceApi,
        RestGetDeficitTemplate,
        LoginTime,
        OpManager,
        RestGetDeliveryWithPriceTemplate,
        RestGetFacturaTemplate,
        RestGetCashTemplate,
        RestGetDeliveryWithoutPriceTemplate,
        RestCompany,
        AddressChanged,
        LastpricesChanged,
        ArticleChanged,
        ArticlePriceChanged,
        OutstandingpaymentsChanged,
        ActivatorUrl,
        RestGetDocumentsByStateAndTimeframe,
        RestGetOrdersByAccountNumber,
    }

    public class CSettingService : CBaseService, ISettingService, IBaseService
    {
        #region public properties

        public const bool Devel = true;
        public const string SettingsDb = "pssettings.db";

        private const string RestProtocol = "https";
        private const string SubUrlRestDataApi = "api/v1/pocketseller";
        private const string SubUrlRestLoginApi = "api/v1/login";

        private const string SubUrlRestResourceSource = "psweb";
        private const string SubUrlRestSourceApi = SubUrlRestResourceSource + "/" + "api";
        private const string SubUrlRestGetSource = "source/GetSources";
        private const string SubUrlRestGetKey = "source/GetKey";

        private const string SubUrlRestCompany = "GetCompany";

        private const string SubUrlRestAddress = "GetAllAdresses";
        private const string SubUrlRestLastprices = "GetAllLastprices";
        private const string SubUrlRestArticle = "GetAllArticles";
        private const string SubUrlRestArticlePrice = "GetAllArticleprices";
        private const string SubUrlRestOutstandingpayments = "GetAllOpenPayments";

        private const string SubUrlRestAddressChanged = "GetLastChangedAdresses";
        private const string SubUrlRestLastpricesChanged = "GetLastChangedLastprices";
        private const string SubUrlRestArticleChanged = "GetLastChangedArticles";
        private const string SubUrlRestArticlePriceChanged = "GetLastChangedArticleprices";
        private const string SubUrlRestOutstandingpaymentsChanged = "GetLastchangedOpenPayments";

        private const string SubUrlRestDocumentAddOrUpdate = "AddOrUpdateOrder";
        private const string SubUrlRestDocumentInsertOrderAsCreditNote = "InsertOrderAsCreditNote";
        private const string SubUrlRestDocumentInsertOrderAsFactura = "InsertOrderAsFactura";
        private const string SubUrlRestDocumentInsertOrderAsDelivery = "InsertOrderAsDelivery";

        private const string SubUrlRestGetDocumentsByState = "GetOrdersByState";
        private const string SubUrlRestGetOrdersByStateAndTimeframe = "GetOrdersByStateAndTimeframe";
        private const string SubUrlRestGetOrdersByAccountNumber = "GetOrdersByAccountNumber";
        private const string SubUrlRestDeleteDocument = "DeleteOrderByOrderNumber";
        private const string SubUrlRestChangeDocumentState = "ChangeOrderStateByOrderNumber";
        private const string SubUrlRestExportToErp= "ExportToErp";
        private const string SubUrlRestImportToErpAsDelivery = "ImportToErpByOrderByOrderNumber";
        private const string SubUrlRestImportToErpAsFactura = "ImportToErpByOrderByOrderNumber";
        private const string SubUrlRestGetLatestOrdernumber = "GetLatestOrderNumber";

        private const string SubUrlRestQuotationAddOrUpdate = "AddUpdateQuotation";

        private const string SubUrlRestGetQuotationsByState = "GetQuotationsByState";
        private const string SubUrlRestDeleteQuotation = "DeleteQuotationByQuotationNumber";
        private const string SubUrlRestChangeQuotationState = "ChangeQuotationStateByQuotationNumber";
        private const string SubUrlRestGetLatestQuotationnumber = "GetLatestQuotationNumber";

        private const string SubUrlRestGetComissionTemplate = "comission.template.xls";

        private const string SubUrlRestGetCashTemplate = "cash.template.xls";
        private const string SubUrlRestGetFacturaTemplate = "factura.template.xls";
        private const string SubUrlRestGetDeliveryWithPriceTemplate = "deliverynote_with.template.xls";
        private const string SubUrlRestGetDeliveryWithoutPriceTemplate = "deliverynote_without.template.xls";
        private const string SubUrlRestGetDeficitTemplate = "deficit.template.xls";

        private const string ActivatorUrl = @"https://activator.yilmazfeinkost.de:7979";

        protected CDataService DataService { get; set; }

        #endregion

        #region constructors

        public CSettingService(IMvxMessenger objMessenger, IDataService objDataService)
            : base(objMessenger)
        {
            LogTag = GetType().Name;
            DataService = (CDataService)objDataService;
        }

        #endregion

        #region public methods

        public void InitSettings()
        {
            Set(ESettingType.RestGetComissionTemplate, SubUrlRestGetComissionTemplate);
            Set(ESettingType.RestGetCashTemplate, SubUrlRestGetCashTemplate);
            Set(ESettingType.RestGetFacturaTemplate, SubUrlRestGetFacturaTemplate);
            Set(ESettingType.RestGetDeliveryWithPriceTemplate, SubUrlRestGetDeliveryWithPriceTemplate);
            Set(ESettingType.RestGetDeliveryWithoutPriceTemplate, SubUrlRestGetDeliveryWithoutPriceTemplate);
            Set(ESettingType.RestGetDeficitTemplate, SubUrlRestGetDeficitTemplate);

            if (Get<int>(ESettingType.Init) == 0)
            {
                //Default values for importing data
                Set(ESettingType.RestProtocol, RestProtocol);
                Set(ESettingType.RestDataApi, SubUrlRestDataApi);
                Set(ESettingType.RestLoginApi, SubUrlRestLoginApi);
                Set(ESettingType.RestSourceApi, SubUrlRestSourceApi);

                Set(ESettingType.RestCompany, SubUrlRestCompany);
                Set(ESettingType.RestAddress, SubUrlRestAddress);
                Set(ESettingType.RestArticle, SubUrlRestArticle);
                Set(ESettingType.RestOutstandingpayments, SubUrlRestOutstandingpayments);
                Set(ESettingType.RestLastprices, SubUrlRestLastprices);
                Set(ESettingType.RestArticlePrice, SubUrlRestArticlePrice);

                Set(ESettingType.AddressChanged, SubUrlRestAddressChanged);
                Set(ESettingType.LastpricesChanged, SubUrlRestLastpricesChanged);
                Set(ESettingType.ArticleChanged, SubUrlRestArticleChanged);
                Set(ESettingType.ArticlePriceChanged, SubUrlRestArticlePriceChanged);
                Set(ESettingType.OutstandingpaymentsChanged, SubUrlRestOutstandingpaymentsChanged);

                Set(ESettingType.RestDocumentAddOrUpdate, SubUrlRestDocumentAddOrUpdate);
                Set(ESettingType.RestDocumentInsertOrderAsCreditNote, SubUrlRestDocumentInsertOrderAsCreditNote);
                Set(ESettingType.RestDocumentInsertOrderAsDelivery, SubUrlRestDocumentInsertOrderAsDelivery);
                Set(ESettingType.RestDocumentInsertOrderAsFactura, SubUrlRestDocumentInsertOrderAsFactura);
                Set(ESettingType.RestGetDocumentsByState, SubUrlRestGetDocumentsByState);
                Set(ESettingType.RestGetDocumentsByStateAndTimeframe, SubUrlRestGetOrdersByStateAndTimeframe);
                Set(ESettingType.RestGetOrdersByAccountNumber, SubUrlRestGetOrdersByAccountNumber);
                Set(ESettingType.RestChangeDocumentState, SubUrlRestChangeDocumentState);
                Set(ESettingType.RestDeleteDocument, SubUrlRestDeleteDocument);
                Set(ESettingType.RestExportToErp, SubUrlRestExportToErp);
                Set(ESettingType.RestImportToErpAsDelivery, SubUrlRestImportToErpAsDelivery);
                Set(ESettingType.RestImportToErpAsFactura, SubUrlRestImportToErpAsFactura);
                Set(ESettingType.RestGetLatestOrdernumber, SubUrlRestGetLatestOrdernumber);
                Set(ESettingType.RestQuotationAddOrUpdate, SubUrlRestQuotationAddOrUpdate);
                Set(ESettingType.RestGetQuotationsByState, SubUrlRestGetQuotationsByState);
                Set(ESettingType.RestChangeQuotationState, SubUrlRestChangeQuotationState);
                Set(ESettingType.RestDeleteQuotation, SubUrlRestDeleteQuotation);
                Set(ESettingType.RestGetLatestQuotationnumber, SubUrlRestGetLatestQuotationnumber);
                Set(ESettingType.RestGetSources, SubUrlRestGetSource);
                Set(ESettingType.RestGetKey, SubUrlRestGetKey);

                Set(ESettingType.DataSourceId, 1);

                Set(ESettingType.Language, ELanguage.de_DE);

                Set(ESettingType.KeyboardTypeOnSearch, 0);
                Set(ESettingType.KeyboardTypeInDocumentdetail, 2);
                Set(ESettingType.SearchType, 0);
                Set(ESettingType.SearchTypeAddress, 0);
                Set(ESettingType.SearchTypeArticle, 0);
                Set(ESettingType.SearchMaxChar, 3);

                Set(ESettingType.ActivatorUrl, ActivatorUrl);

                Set(ESettingType.Init, 1);
            }
        }

        public void Set<T>(ESettingType enmSetting, T objValue)
        {
            lock (Lock)
            {
                AddValue(enmSetting.ToString(), objValue);
            }
        }

        public T Get<T>(ESettingType enmSetting)
        {
            object objValue = null;

            try
            {
                lock (Lock)
                {
                    string strKey = enmSetting.ToString();
                    Settings objSetting = Settings.Instance.FindSetting(strKey);

                    if (objSetting != null)
                    {
                        if (typeof(T) == typeof(string))
                        {
                            objValue = objSetting.Value;
                        }
                        else if (typeof(T) == typeof(int))
                        {
                            objValue = Convert.ToInt32(objSetting.Value);
                        }
                        else if (typeof(T) == typeof(double))
                        {
                            objValue = (Convert.ToDouble(objSetting.Value) / 1000000);
                        }
                        else if (typeof(T) == typeof(decimal))
                        {
                            objValue = Convert.ToDecimal(objSetting.Value);
                        }
                        else if (typeof(T) == typeof(DateTime))
                        {
                            objValue = DateTime.Parse(objSetting.Value, CultureInfo.InvariantCulture);
                        }
                        else if (typeof(T) == typeof(ELanguage))
                        {
                            objValue = Enum.Parse(typeof(ELanguage), objSetting.Value, false);
                        }
                        else if (typeof(T) == typeof(ListSortDirection))
                        {
                            objValue = Enum.Parse(typeof(ListSortDirection), objSetting.Value, false);
                        }
                    }

                    return objValue != null ? (T)objValue : Activator.CreateInstance<T>();
                }
            }
            catch (Exception)
            {
                return (T)objValue;
            }
        }

        #endregion

        #region private methods

        private void AddValue<T>(string strKey, T objValue)
        {
            try
            {
                var strValue = (typeof(T) == typeof(DateTime))
                    ? Convert.ToDateTime(objValue).ToString("o")
                    : objValue.ToString();

                var objSetting = new Settings { Id = Guid.NewGuid(), Key = strKey, Value = strValue };

                var bKeyNotFound = (Settings.Instance.FindSetting(strKey) == null);

                if (bKeyNotFound)
                {
                    Settings.Instance.Insert(objSetting);
                }
                else
                {
                    var objFoundSetting = Settings.Instance.FindSetting(strKey);
                    if (objFoundSetting != null && !objFoundSetting.Value.Equals(objSetting.Value))
                    {
                        objSetting.Id = objFoundSetting.Id;
                        Settings.Instance.Update(objSetting);
                    }
                    else if (objFoundSetting == null)
                    {
                        Settings.Instance.Insert(objSetting);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

        }

        #endregion
    }
}
