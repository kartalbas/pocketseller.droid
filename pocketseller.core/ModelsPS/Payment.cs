using System;
using SQLite;

namespace pocketseller.core.Models
{
    public enum EPaymentMode
    {
        Barzahlung = 1,
        Scheck = 2,
        Ueberweisung = 3,
        Lastschrift = 4,
        Wechsel = 5,
        Rueckscheck = 6,
        Skonto_Rabatt = 7,
        Verrechnung = 8,
        Korrektur = 9
    }

    public class Payment : BaseModel
    {
        public Payment()
        {
            LogTag = GetType().Name;            
        }

        private int _AddressNr;
        [Indexed]
        public int AddressNr { get => _AddressNr;
                set { _AddressNr = value; RaisePropertyChanged(() => AddressNr); } }

        private int _paymentMode;
        public int PaymentMode { get => _paymentMode;
                set { _paymentMode = value; RaisePropertyChanged(() => PaymentMode); } }

        private DateTime _paymentDate;
        public DateTime PaymentDate { get => _paymentDate;
                set { _paymentDate = value; RaisePropertyChanged(() => PaymentDate); } }

        private decimal _paymentAmount;
        public decimal PaymentAmount { get => _paymentAmount;
                set { _paymentAmount = value; RaisePropertyChanged(() => PaymentAmount); } }

        private int _chequeNr;
        public int ChequeNr { get => _chequeNr;
                set { _chequeNr = value; RaisePropertyChanged(() => ChequeNr); } }

        private string _File;
        [Indexed]
        public string File { get => _File;
                set { _File = value; RaisePropertyChanged(() => File); } }

        private int _fileId;
        [Indexed]
        public int FileId { get => _fileId;
                set { _fileId = value; RaisePropertyChanged(() => FileId); } }

        public static string GetPaymentMode(EPaymentMode enmPaymentMode)
        {
            switch (enmPaymentMode)
            {
                    case EPaymentMode.Barzahlung:
                            return "BAR";
                    case EPaymentMode.Korrektur:
                            return "KOR";
                    case EPaymentMode.Lastschrift:
                            return "LAST";
                    case EPaymentMode.Rueckscheck:
                            return "RÜK";
                    case EPaymentMode.Scheck:
                            return "SCH";
                    case EPaymentMode.Skonto_Rabatt:
                            return "SKO";
                    case EPaymentMode.Ueberweisung:
                            return "ÜBW";
                    case EPaymentMode.Verrechnung:
                            return "VER";
                    case EPaymentMode.Wechsel:
                            return "WEL";
                    default:
                            return string.Empty;
            }
        }
    }
}
