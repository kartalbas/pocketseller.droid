using System;
using SQLite;

namespace pocketseller.core.Models
{
    public class Paymentdetails : BaseModel
    {
        public Paymentdetails()
        {
            LogTag = GetType().Name;
        }

        private int _paymentId;
        [Indexed]
        public int PaymentId { get => _paymentId;
            set { _paymentId = value; RaisePropertyChanged(() => PaymentId); } }

        private int _invoiceNr;
        public int InvoiceNr { get => _invoiceNr;
            set { _invoiceNr = value; RaisePropertyChanged(() => InvoiceNr); } }

        private DateTime _invoiceDate;
        public DateTime InvoiceDate { get => _invoiceDate;
            set { _invoiceDate = value; RaisePropertyChanged(() => InvoiceDate); } }

        private decimal _invoiceAmount;
        public decimal InvoiceAmount { get => _invoiceAmount;
            set { _invoiceAmount = value; RaisePropertyChanged(() => InvoiceAmount); } }

        private string _File;
        [Indexed]
        public string File { get => _File;
            set { _File = value; RaisePropertyChanged(() => File); } }

        private int _fileId;
        [Indexed]
        public int FileId { get => _fileId;
            set { _fileId = value; RaisePropertyChanged(() => FileId); } }
    }
}
