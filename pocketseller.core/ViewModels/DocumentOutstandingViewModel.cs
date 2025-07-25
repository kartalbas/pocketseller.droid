﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Models;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class DocumentOutstandingViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public DocumentOutstandingViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
        }

        #endregion

        #region Private methods

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.TabOustanding;
            LabelInvoiceDate = Language.InvoiceDate;
            LabelInvoiceNumber = Language.InvoiceNumber;
            LabelInvoiceOpen = Language.InvoiceOpen;
            LabelInvoiceValue = Language.InvoiceValue;
            LabelTotalOpen = Language.TotalOpen;
            ShowOoutstandingPayments();
        }

        public override void Init(object objParam) { }

        #endregion

        #region Public properties

        public void ShowOoutstandingPayments()
        {
            ListOutstandingpayments = new ObservableCollection<OpenPayment>();
            if (Adress != null)
                ListOutstandingpayments = OpenPayment.Find(Adress);
        }
        
        public string LabelPayed => Language.Payed;

        
        private string _labelTotalOpen;
        public string LabelTotalOpen { get => _labelTotalOpen;
            set { _labelTotalOpen = value; RaisePropertyChanged(() => LabelTotalOpen); } }

        public decimal TextTotalOpen => OpenPayment.GetTotalOpen(Adress);

        private string _labelInvoiceNumber;
        public string LabelInvoiceNumber
        {
            get => _labelInvoiceNumber;
            set { _labelInvoiceNumber = value; RaisePropertyChanged(() => LabelInvoiceNumber); }
        }

        private string _labelInvoiceDate;
        public string LabelInvoiceDate
        {
            get => _labelInvoiceDate;
            set { _labelInvoiceDate = value; RaisePropertyChanged(() => LabelInvoiceDate); }
        }

        private string _labelInvoiceValue;
        public string LabelInvoiceValue
        {
            get => _labelInvoiceValue;
            set { _labelInvoiceValue = value; RaisePropertyChanged(() => LabelInvoiceValue); }
        }

        private string _labelInvoiceOpen;
        public string LabelInvoiceOpen
        {
            get => _labelInvoiceOpen;
            set { _labelInvoiceOpen = value; RaisePropertyChanged(() => LabelInvoiceOpen); }
        }

        public Adress Adress => DocumentService?.Document?.Adress;

        private ObservableCollection<OpenPayment> _listOutstandingpayments;
        public ObservableCollection <OpenPayment> ListOutstandingpayments
        {
            get => _listOutstandingpayments;
            set
            {
                DoShowWorkingCommand();
                _listOutstandingpayments = value;
                RaisePropertyChanged(() => ListOutstandingpayments);                
                DoHideWorkingCommand();
            }
        }

        #endregion

        #region ICommand implementations
        
        private MvxCommand<OpenPayment> _deleteDocumentCommand;
        public ICommand MarkAsPayedCommand { get { return _deleteDocumentCommand = _deleteDocumentCommand ?? new MvxCommand<OpenPayment>(DoMarkAsPayedCommand); } }
        private async void DoMarkAsPayedCommand(OpenPayment op)
        {
            try
            {
                DoShowWorkingCommand();
                    
                if (op != null)
                {                
                    //check if already an answer exists
                    var rest = Mvx.IoCProvider.Resolve<IRestService>();
                    var mailItems = await rest.GetMails();
                    foreach (var mailItem in mailItems)
                    {
                        if(mailItem.Subject.Contains(op.Adressnumber)
                           && mailItem.Subject.Contains(op.Docnumber))
                        {
                            if (mailItem.Body.ToLower().StartsWith("ok"))
                            {
                                OpenPayment.Delete(op);
                                await rest.DeleteMail(mailItem.MessageId);
                                await rest.InsertDeletedOpenPayment(op.Adressnumber, op.Docnumber);
                                ShowOoutstandingPayments();
                                return;
                            }
                        }
                    }

                    //if no answer exists, send an mail
                    var currentSource = Source.Instance.GetCurrentSource();
                    var konto = Adress.FindByKontoNr(op.Adressnumber);
                    var externalId = SettingService.Get<string>(ESettingType.ExternalId);
                    var username = SettingService.Get<string>(ESettingType.Username);

                    var subject = $"KONTO: {op.Adressnumber} - RENR:{op.Docnumber} - REDT:{op.Docdate:d}";
                    var body = "<br/>";
                    body = body + "*****************************<br/>";
                    body = body + $"USERNAME: {username}<br/>";
                    body = body + $"ERP USER: {externalId}<br/>";
                    body = body + $"LAGER: {currentSource?.Name}<br/>";
                    body = body + "*****************************<br/>";
                    body = body + "<br/>";
                    body = body + "*****************************<br/>";
                    body = body + $"KONTO: {op.Adressnumber}<br/>";
                    body = body + $"{konto?.Name1}, {konto?.Name2}<br/>"; 
                    body = body + $"{konto?.Street}<br/>";
                    body = body + $"{konto?.Zip} {konto?.City}<br/>";
                    body = body + $"RE-NR: {op.Docnumber}<br/>";
                    body = body + $"RE-DATUM: {op.Docdate:d}<br/>";
                    body = body + $"RE-BETRAG: {op.Amountorder:F}<br/>";
                    body = body + $"RE-OFFEN: {op.Amountopen:F}<br/>";
                    body = body + "*****************************<br/>";
                    body = body + "<br/>";

                    //var to = "kartalbas@gmail.com";
                    var to = SettingService.Get<string>(ESettingType.OpManager);
                    await rest.SendMail(null, to, subject, body);
                }

                DoHideWorkingCommand();
            }
            catch (Exception exception)
            {
                DoHideWorkingCommand();
                LogError(exception);
            }
        }

        #endregion
    }
}
