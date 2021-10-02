using System;

namespace pocketseller.core.Models
{
    public class Invoices : BaseModel
    {
        public Invoices()
        {
            LogTag = GetType().Name;
        }

        private int _addressNr;
        public int AddressNr { get => _addressNr;
            set { _addressNr = value; RaisePropertyChanged(() => AddressNr); } }

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
        public string File { get => _File;
            set { _File = value; RaisePropertyChanged(() => File); } }

        private int _fileId;
        public int FileId { get => _fileId;
            set { _fileId = value; RaisePropertyChanged(() => FileId); } }
    }
}
