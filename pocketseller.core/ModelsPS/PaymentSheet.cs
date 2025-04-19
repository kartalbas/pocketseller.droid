using System;

namespace pocketseller.core.Models
{
    public class PaymentSheet
    {
        public int PaymentId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string AccountNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string PaymentAmount { get; set; }
        public decimal? PartialPaymentAmount { get; set; }
        public string PaymentType { get; set; }
    }
}
