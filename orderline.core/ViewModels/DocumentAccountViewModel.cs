using System;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Models;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentAccountViewModel : BaseViewModel
    {
        public DocumentAccountViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
        }

        #region Private methods

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methiods

        public override void Init()
        {
            LabelTitle = Language.TabAccount;
            LabelPaymentDate = Language.PaymentDate;
            LabelChequeNr = Language.CheckqueNr;
            LabelPaymentText = Language.PaymentText;
            LabelInvoiceDate = Language.InvoiceDate;
            LabelInvoiceNr = Language.InvoiceNr;
            LabelPayment = Language.Payment;
            LabelCredit = Language.Credit;
            LabelDebit = Language.Debit;
            LabelAveragePaymentCraftInDays = Language.LabelAveragePaymentCraftInDays;
        }

        public override void Init(object objParam) {}

        #endregion

        #region Public properties

        private string _labelChequeNr;
        public string LabelChequeNr { get => _labelChequeNr;
            set { _labelChequeNr = value; RaisePropertyChanged(() => LabelChequeNr); } }

        private string _labelCredit;
        public string LabelCredit { get => _labelCredit;
            set { _labelCredit = value; RaisePropertyChanged(() => LabelCredit); } }

        private string _labelDebit;
        public string LabelDebit { get => _labelDebit;
            set { _labelDebit = value; RaisePropertyChanged(() => LabelDebit); } }

        private string _labelInvoiceDate;
        public string LabelInvoiceDate { get => _labelInvoiceDate;
            set { _labelInvoiceDate = value; RaisePropertyChanged(() => LabelInvoiceDate); } }

        private string _labelInvoiceNr;
        public string LabelInvoiceNr { get => _labelInvoiceNr;
            set { _labelInvoiceNr = value; RaisePropertyChanged(() => LabelInvoiceNr); } }

        private string _labelPayment;
        public string LabelPayment { get => _labelPayment;
            set { _labelPayment = value; RaisePropertyChanged(() => LabelPayment); } }

        private string _labelPaymentDate;
        public string LabelPaymentDate { get => _labelPaymentDate;
            set { _labelPaymentDate = value; RaisePropertyChanged(() => LabelPaymentDate); } }

        private string _labelPaymentTextPaymentText;
        public string LabelPaymentText { get => _labelPaymentTextPaymentText;
            set { _labelPaymentTextPaymentText = value; RaisePropertyChanged(() => LabelPaymentText); } }

        public Adress Adress => DocumentService.Document.Adress;

        private decimal _averagePaymentCraftInDays;
        public decimal AveragePaymentCraftInDays { get => _averagePaymentCraftInDays;
            set { _averagePaymentCraftInDays = value; RaisePropertyChanged(() => AveragePaymentCraftInDays); } }

        private string _labelAveragePaymentCraftInDays;
        public string LabelAveragePaymentCraftInDays { get => _labelAveragePaymentCraftInDays;
            set { _labelAveragePaymentCraftInDays = value; RaisePropertyChanged(() => LabelAveragePaymentCraftInDays); } }

        public ObservableCollection<DocumentAccountItem> ListAccountInfo
        {
            get
            {
                int iAddressNr = Convert.ToInt32(Adress.Adressnumber);
                if (iAddressNr > 0)
                {
                    DoShowWorkingCommand();

                    //AveragePaymentCraftInDays = GetAveragePaymentCraft(iAddressNr);
                    var objAccountInfo = GetAccountInfo(iAddressNr);

                    DoHideWorkingCommand();

                    return objAccountInfo;
                }

                return null;
            }
        }

        private decimal GetAveragePaymentCraft(int iAddressNr)
        {
            try
            {
                //Get all invoices
                var objInvoices = DataService.PocketsellerConnection
                    .Table<Invoices>()
                    .Where(a => a.AddressNr == iAddressNr)
                    .ToList();

                int iCountInvoices = objInvoices.Count;
                decimal iSumDays = 0;

                foreach (var objInvoice in objInvoices)
                {
                    var iInvoiceNr = objInvoice.InvoiceNr;

                    var objPaymentdetails = DataService.PocketsellerConnection
                        .Table<Paymentdetails>()
                        .Where(a => a.InvoiceNr == iInvoiceNr)
                        .ToList();

                    var iInvoiceDays = 0;
                    foreach (var objPaymentdetail in objPaymentdetails)
                    {
                        var strPaymentFile = objPaymentdetail.File;
                        var iPaymentFileId = objPaymentdetail.PaymentId + 1;

                        //Get all payment details for that payment
                        var objPayment = (DataService.PocketsellerConnection
                            .Table<Payment>()
                            .Where(a => a.File == strPaymentFile)
                            .Where(a => a.FileId == iPaymentFileId))
                            .FirstOrDefault();

                        if (objPayment != null)
                            iInvoiceDays += objPayment.PaymentDate.Subtract(objInvoice.InvoiceDate).Days;
                    }

                    if (objPaymentdetails.Count > 0)
                        iSumDays += iInvoiceDays / objPaymentdetails.Count;
                }

                if (iCountInvoices > 0)
                    return iSumDays/iCountInvoices;

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private ObservableCollection<DocumentAccountItem> GetAccountInfo(int iAddressNr)
        {
            try
            {
                var objAccountinfo = new ObservableCollection<DocumentAccountItem>();

                //Get all invoices
                var objInvoices = DataService.PocketsellerConnection
                    .Table<Invoices>()
                    .Where(a => a.AddressNr == iAddressNr)
                    .ToList();

                //Add the invoices in collection
                foreach (var objInvoice in objInvoices)
                {
                    objAccountinfo.Add(new DocumentAccountItem
                    {
                        PaymentDate = objInvoice.InvoiceDate,
                        ChequeNr = "",
                        PaymentText = "RE",
                        InvoiceDate = default(DateTime),
                        InvoiceNr = objInvoice.InvoiceNr,
                        Payment = 0,
                        Credit = objInvoice.InvoiceAmount,
                        Debit = 0
                    });
                }

                //Get all payments
                var objPayments = DataService.PocketsellerConnection.Table<Payment>()
                    .Where(a => a.AddressNr == iAddressNr).ToList();

                //Iterate through all payments
                foreach (var objPayment in objPayments)
                {
                    var strPaymentFile = objPayment.File;
                    //TODO: adapt this change in gmdb! this was formaly the row number of zabmmjj, need to begin with 0
                    var iPaymentFileId = objPayment.FileId + 1;

                    //Get all payment details for that payment
                    var objPaymentdetails = DataService.PocketsellerConnection
                        .Table<Paymentdetails>()
                        .Where(a => a.File == strPaymentFile)
                        .Where(a => a.PaymentId == iPaymentFileId)
                        .OrderBy(a => a.InvoiceDate).ToList();
                    //Combinate Payment + Paymentdetails and add them to the collection
                    foreach (var objPaymentdetail in objPaymentdetails)
                    {
                        objAccountinfo.Add(new DocumentAccountItem
                        {
                            PaymentDate = objPayment.PaymentDate,
                            ChequeNr = objPayment.ChequeNr.ToString(),
                            PaymentText = Payment.GetPaymentMode((EPaymentMode) objPayment.PaymentMode),
                            InvoiceDate = objPaymentdetail.InvoiceDate,
                            InvoiceNr = objPaymentdetail.InvoiceNr,
                            Payment = objPayment.PaymentAmount,
                            Credit = 0,
                            Debit = objPayment.PaymentAmount
                        });
                    }
                }

                var cobjOrderedAccount = objAccountinfo.OrderBy(item => item.PaymentDate);
                var objResult = new ObservableCollection<DocumentAccountItem>(cobjOrderedAccount);

                var dSumCredit = cobjOrderedAccount.Sum(c => c.Credit);
                var dSumDebit = cobjOrderedAccount.Sum(d => d.Debit);
                objResult.Add(new DocumentAccountItem
                {
                    PaymentDate = default(DateTime),
                    ChequeNr = "",
                    PaymentText = Language.Total,
                    InvoiceDate = default(DateTime),
                    InvoiceNr = 0,
                    Payment = 0,
                    Credit = dSumCredit,
                    Debit = dSumDebit
                });

                DoHideWorkingCommand();

                return objResult;
            }
            catch (Exception)
            {
                return null;
            }
        }
            
        public class DocumentAccountItem
        {
            public DateTime PaymentDate { get; set; }
            public string ChequeNr { get; set; }
            public string PaymentText { get; set; }
            public DateTime InvoiceDate { get; set; }
            public int InvoiceNr { get; set; }
            public decimal Payment { get; set; }
            public decimal Credit { get; set; }
            public decimal Debit { get; set; }
        }

        #endregion
    }
}
