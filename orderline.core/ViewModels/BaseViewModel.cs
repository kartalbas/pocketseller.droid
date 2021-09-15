using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using pocketseller.core.Messages;
using pocketseller.core.ModelConverter;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using Quotation = pocketseller.core.Models.Quotation;
using orderline.core.Resources;
using FlexCel.Core;
using orderline.core.ModelsPS;

namespace pocketseller.core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        #region Protected properties
        public delegate void OnRemoteDocumentChangedHandler(EOrderState enmDocumentState, BaseViewModel objViewModel, DateTime? begin = null, DateTime? end = null, string adressNumber = null);
        public event OnRemoteDocumentChangedHandler OnRemoteDocumentChanged;

        protected IMvxNavigationService NavigationService { get; set; }
        public CSettingService SettingService { get; private set; }
        public CDataService DataService { get; private set; }
        protected CDocumentService DocumentService { get; private set; }
        protected CLanguageService LanguageService { get; private set; }
        protected CSingletonService SingletonService { get; private set; }
        protected MvxSubscriptionToken SubscriptionToken1 { get; set; }
        protected MvxSubscriptionToken SubscriptionToken2 { get; set; }
        protected MvxSubscriptionToken SubscriptionToken3 { get; set; }
        protected IMvxMessenger Messenger { get; private set; }

        protected string LogTag { get; set; }
        protected object Lock { get; private set; }
        #endregion

        #region Constructors

        public BaseViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService , IMvxMessenger objMessenger)
        {
            Lock = new object();
            SettingService = (CSettingService)objSettingService;
            DataService = (CDataService)objDataService;
            DocumentService = (CDocumentService)objDocumentService;
            LanguageService = (CLanguageService)objLanguageService;
            SingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();
            NavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            Messenger = objMessenger;
        }

        protected virtual void RemoteDocumentChanged(EOrderState enmDocumentState, BaseViewModel objViewModel)
        {
            var objHandler = OnRemoteDocumentChanged;
            if (objHandler != null)
                objHandler(enmDocumentState, objViewModel);
        }

        public abstract void Init();
        public abstract void Init(object objParam);

        protected void LogError(Exception objException)
        {
            DoHideWorkingCommand();

            while (objException.InnerException != null)
            {
                objException = objException.InnerException;
            }

            Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(objException.Message, Language.Attention, Language.Ok);
        }

        protected string GetMessageFromException(Exception exception)
        {
            try
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                var lines = exception.Message.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                var message = lines.FirstOrDefault(a => a.ToLower().StartsWith("message"));
                if(!string.IsNullOrEmpty(message))
                {
                    var messages = message.Split(new[] {":"}, StringSplitOptions.None);
                    if (messages.Length > 1)
                    {
                        return messages[1].Trim();
                    }                            
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }
        }

        private string _labelTitle;

        public string LabelTitle
        {
            get => _labelTitle;
            set
            {
                if (value.StartsWith("$"))
                {
                    string strTitle = value.Replace("$", "");
                    strTitle = value.Contains("(") ? Regex.Match(value, @"[A-Za-z0-9äöüßÄÖÜ]+[^\(\w\)]").Value.Trim() : strTitle;

                    var objCurrentSource = Source.Instance.GetCurrentSource();
                    string strName = objCurrentSource == null ? string.Empty : objCurrentSource.Name;

                    if (string.IsNullOrEmpty(strName))
                        strName = Language.NotActivated;

                    _labelTitle = string.Format("{0} ({1})", strTitle, strName);
                    Messenger.Publish(new TitleMessage(this, ETitleAction.Changed));
                }
                else
                {
                    _labelTitle = value;
                }

                RaisePropertyChanged(() => LabelTitle);
            }
        }

        private string _labelMenuTitle;
        public string LabelMenuTitle { get => _labelMenuTitle;
            set { _labelMenuTitle = value; RaisePropertyChanged(() => LabelMenuTitle); } }

        public ObservableCollection<Document> _documents;
        public ObservableCollection<Document> Documents { get => _documents;
            set { _documents = value; RaisePropertyChanged(() => Documents); } }

        public ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders { get => _orders;
            set { _orders = value; RaisePropertyChanged(() => Orders); } }

        public ObservableCollection<Quotation> _quotations;
        public ObservableCollection<Quotation> Quotations { get => _quotations;
            set { _quotations = value; RaisePropertyChanged(() => Quotations); } }

        protected async Task<bool> PrintOrder(Order order, string reportTemplateFileName)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var reportService = Mvx.IoCProvider.Resolve<IReportService>();
                    var printService = Mvx.IoCProvider.Resolve<IPrintService>();
                    var restService = Mvx.IoCProvider.Resolve<IRestService>();

                    var fileName = string.Empty;
                    var date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
                    var targetPdfFileName = $"{order.Adressnumber}_{date}.pdf";

                    order.FacturaData = await restService.GetFacturaDataAsync(order.Docnumber);
                    order.Orderdetails = order.FacturaData.Order.Orderdetails.OrderBy(o => o.Pos).ToList();

                    fileName = reportService.CreateReport(order, reportTemplateFileName, targetPdfFileName);

                    printService.PrintNative(fileName);

                });

                return true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                return false;
            }
        }

        protected async Task<bool> PrintLocalOrder(Order order, string reportTemplateFileName)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var reportService = Mvx.IoCProvider.Resolve<IReportService>();
                    var printService = Mvx.IoCProvider.Resolve<IPrintService>();
                    var restService = Mvx.IoCProvider.Resolve<IRestService>();

                    var fileName = string.Empty;
                    var date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
                    var targetPdfFileName = $"{order.Adressnumber}_{date}.pdf";

                    order.Orderdetails = order.Orderdetails.OrderBy(o => o.Pos).ToList();
                    order.FacturaData = await restService.GetFacturaDataAsync(1);

                    fileName = reportService.CreateLocalReport(order, reportTemplateFileName, targetPdfFileName);

                    printService.PrintNative(fileName);

                });

                return true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                return false;
            }
        }

        protected async Task<bool> PrintOrders(List<Order> orders)
        {
            try
            {

                if (orders == null || orders.Count() <= 0)
                    return true;

                var result = new List<ExcelFile>();

                await Task.Run(async () =>
                {
                    var restService = Mvx.IoCProvider.Resolve<IRestService>();
                    var reportService = Mvx.IoCProvider.Resolve<IReportService>();
                    var printService = Mvx.IoCProvider.Resolve<IPrintService>();

                    foreach (var order in orders)
                    {
                        order.FacturaData = await restService.GetFacturaDataAsync(order.Docnumber);
                        var excelReport = reportService.CreateExcelReport(order);
                        result.Add(excelReport);
                    }

                    var date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
                    var targetPdfFileName = $"FACTURAS_{date}.pdf";
                    var fileName = reportService.ExportReportAsPdf(result, targetPdfFileName);

                    printService.PrintNative(fileName);
                });

                return true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                return false;
            }
        }


        protected async Task<bool> MailOrder(Order order, string reportTemplateFileName)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var report = Mvx.IoCProvider.Resolve<IReportService>();
                    var mail = Mvx.IoCProvider.Resolve<IMailService>();
                    var source = Source.Instance.GetCurrentSource();
                    var date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
                    var targetPdfFileName = $"{order.Adressnumber}_{date}.pdf";

                    var restService = Mvx.IoCProvider.Resolve<IRestService>();
                    order.FacturaData = await restService.GetFacturaDataAsync(1);
                    var filename = report.CreateLocalReport(order, reportTemplateFileName, targetPdfFileName);

                    mail.ShowDraft(order.FacturaData.Company.Subject, order.FacturaData.Company.Body, true, string.Empty, new List<string> { filename });
                });

                return true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                return false;
            }
        }

        private MvxCommand _showWorkingCommand;
        public ICommand ShowWorkingCommand { get { _showWorkingCommand = _showWorkingCommand ?? new MvxCommand(DoShowWorkingCommand); return _showWorkingCommand; } }
        protected void DoShowWorkingCommand()
        {
            Messenger.Publish(new WorkingMessage(this, EWorkingAction.ShowWorking));
        }

        private MvxCommand _hideWorkingCommand;
        public ICommand HideWorkingCommand { get { _hideWorkingCommand = _hideWorkingCommand ?? new MvxCommand(DoHideWorkingCommand); return _hideWorkingCommand; } }
        protected void DoHideWorkingCommand()
        {
            Messenger.Publish(new WorkingMessage(this, EWorkingAction.HideWorking));
        }

        protected void HandleResult(Order objDocument, EOrderView enmOrderView)
        {
            if (objDocument != null)
            {
                if (objDocument.Response.Replace("\"", "") == Globals.TRUE)
                {
                    //Messenger.Publish(new DocumentsViewServiceMessage(this, enmDocumentsViewAction));
                }
                else
                {
                    string strMessage = Language.StatusNotChanged;
                    strMessage += Environment.NewLine;
                    strMessage += "ERROR MESSAGE: ";
                    strMessage += objDocument.Response;
                    strMessage += Environment.NewLine;
                    Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(strMessage, Language.Attention);
                }
            }
            else
            {
                string strMessage = Language.StatusNotChanged;
                strMessage += Environment.NewLine;
                strMessage += "ERROR MESSAGE: Document = NULL";
                strMessage += Environment.NewLine;
                Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(strMessage, Language.Attention);
            }

            Messenger.Publish(new OrdersViewServiceMessage(this, enmOrderView));
        }

        public bool CheckLoginTimeOut
        {
            get
            {
                var objLoginDate = SettingService.Get<DateTime>(ESettingType.LoginTime);

                if (DateTime.Now.Subtract(objLoginDate).TotalMinutes >= (60 * 12))
                {
                    objLoginDate = default(DateTime);
                    SettingService.Set(ESettingType.LoginTime, default(DateTime));
                }

                return objLoginDate != default(DateTime);
            }
        }

        private MvxCommand _logoutCommand;
        public ICommand LogoutCommand { get { _logoutCommand = _logoutCommand ?? new MvxCommand(DoLogoutCommand); return _logoutCommand; } }
        private void DoLogoutCommand()
        {
            SettingService.Set(ESettingType.LoginTime, default(DateTime));
        }

        private MvxCommand _checkLoginCommand;
        public ICommand CheckLoginCommand { get { _checkLoginCommand = _checkLoginCommand ?? new MvxCommand(DoCheckLoginCommand); return _checkLoginCommand; } }
        private void DoCheckLoginCommand()
        {
            if (!CheckLoginTimeOut)
                NavigationService.Navigate<LoginViewModel>();
        }

        private MvxAsyncCommand<Document> _emailCommand;
        public IMvxAsyncCommand<Document> EmailCommand => _emailCommand = _emailCommand ?? new MvxAsyncCommand<Document>(DoEmailCommand);
        private async Task DoEmailCommand(Document document)
        {
            var source = Source.Instance.GetCurrentSource();
            var order = Converter.CreateOrder(source, document);
            var template = SettingService.Get<string>(ESettingType.RestGetDeliveryWithPriceTemplate);
            await MailOrder(order, template);
        }

        private MvxAsyncCommand<Document> _printDocumentCommand;
        public IMvxAsyncCommand<Document> PrintDocumentCommand => _printDocumentCommand = _printDocumentCommand ?? new MvxAsyncCommand<Document>(DoPrintDocumentCommand);
        private async Task DoPrintDocumentCommand(Document document)
        {
            var source = Source.Instance.GetCurrentSource();
            var order = Converter.CreateOrder(source, document);
            var template = SettingService.Get<string>(ESettingType.RestGetDeliveryWithPriceTemplate);

            if (document.LocalDocument)
            {
                await PrintLocalOrder(order, template);
            }
            else
            {
                await PrintOrder(order, template);
            }
        }

        private MvxAsyncCommand<Order> _printOrderCommand;
        public IMvxAsyncCommand<Order> PrintOrderCommand => _printOrderCommand = _printOrderCommand ?? new MvxAsyncCommand<Order>(DoPrintOrderCommand);
        private async Task DoPrintOrderCommand(Order order)
        {
            await PrintOrder(order, order.Response);
        }

        private MvxAsyncCommand<List<Order>> _printOrdersCommand;
        public IMvxAsyncCommand<List<Order>> PrintOrdersCommand => _printOrdersCommand = _printOrdersCommand ?? new MvxAsyncCommand<List<Order>>(DoPrintOrdersCommand);
        private async Task DoPrintOrdersCommand(List<Order> orders)
        {
            await PrintOrders(orders);
        }

        private MvxCommand<Type> _navigateToCommand;
        public ICommand NavigateToCommand { get { return _navigateToCommand = _navigateToCommand ?? new MvxCommand<Type>(DoNavigateToCommand); } }
        private void DoNavigateToCommand(Type objType)
        {
            NavigationService.Navigate(objType);
        }

        private MvxCommand<Document> _sendDocumentCommand;
        public ICommand SendDocumentCommand { get { return _sendDocumentCommand = _sendDocumentCommand ?? new MvxCommand<Document>(DoSendDocumentCommand); } }
        private void DoSendDocumentCommand(Document objDocument)
        {
            if (objDocument != null)
            {
                int iDocumentNumber = 0;
                int.TryParse(objDocument.Response.Replace("\"", ""), out iDocumentNumber);

                if (iDocumentNumber > 0)
                {
                    Document.ChangePhase(objDocument, EPhaseState.SERVER);
                    OrderSettings.Instance.CurrentDocNr = iDocumentNumber;
                    Document.UpdateDocumentnumber(objDocument, iDocumentNumber);
                    Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
                }
                else
                {
                    Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.OrderCouldNotBeSent + " >>> ERROR MESSAGE: " + objDocument.Response, Language.Attention);
                }
            }
            else
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.OrderCouldNotBeSent + " >>> ERROR MESSAGE: Document = NULL", Language.Attention);
            }
        }

        protected ObservableCollection<core.Models.Quotationdetail> GetAllValidQuotations()
        {
            var quotationdetails = new ObservableCollection<core.Models.Quotationdetail>();
            var quotations = Quotation.FindValid();
            foreach (var quotation in quotations)
            {
                foreach (var quotationdetail in quotation.Quotationdetails)
                {
                    quotationdetails.Add(quotationdetail);
                }
            }

            return quotationdetails;
        }

        #endregion
    }
}
