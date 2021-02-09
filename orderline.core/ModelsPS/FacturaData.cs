using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace orderline.core.ModelsPS
{
    public class FacturaData
    {
        public Adress Adress { get; set; }
        public Company Company { get; set; }
        public Order Order { get; set; }
        public IList<OpenPayment> OpenPayments { get; set; }
        public IList<PaymentSheet> PaymentSheets { get; set; }

        public decimal TotalOpenAmount { get; set; }
        public string Rechnungsnummer { get; set; }
        public string Lieferscheinnummer { get; set; }
        public DateTime? Rechnungsdatum { get; set; }
        public DateTime? Lieferdatum { get; set; }
        public string FacturaText { get; set; }

        public decimal TaxInPercent0 { get; set; }
        public decimal NettoAmount0 { get; set; }
        public decimal BruttoAmount0 { get; set; }
        public decimal TaxAmount0 { get; set; }
        public decimal TaxInPercent1 { get; set; }
        public decimal NettoAmount1 { get; set; }
        public decimal BruttoAmount1 { get; set; }
        public decimal TaxAmount1 { get; set; }
        public decimal TotalAmount { get; set; }

        public static CSettingService SettingService { get; set; }

        public static decimal GetTotalOpenAmount(IList<OpenPayment> openPayment)
        {
            return openPayment.Sum(o => o.Amountopen);
        }    

        public static IList<OpenPayment> FormatOpenPayments(IList<OpenPayment> openPayment)
        {
            if (openPayment.Count() == 0)
            {
                openPayment.Add(new OpenPayment
                {
                    Id = Guid.NewGuid(),
                    Adressnumber = string.Empty,
                    Amountopen = 0,
                    Amountorder = 0,
                    Docdate = null,
                    Docnumber = string.Empty,
                    TimeStamp = DateTime.Now
                });
            }

            return openPayment;
        }

        public static IList<PaymentSheet> FormatPaymentSheet(IList<PaymentSheet> paymentSheets)
        {
            var result = paymentSheets.OrderBy(s => s.PaymentId).ToList();

            var paymentIds = result.Select(s => s.PaymentId).Distinct();

            foreach(var paymentId in paymentIds)
            {
                var skipped = false;
                var sheets = result.Where(s => s.PaymentId == paymentId);
                foreach(var sheet in sheets)
                {
                    if(skipped)
                    {
                        sheet.PaymentAmount = string.Empty;
                        sheet.PaymentType = string.Empty;
                        continue;
                    }
                    skipped = true;
                    sheet.PaymentAmount = Convert.ToDecimal(sheet.PaymentAmount, CultureInfo.InvariantCulture).ToString("F");
                }
            }

            if (paymentIds.Count() == 0)
            {
                paymentSheets.Add(new PaymentSheet
                {
                    PaymentId = 0,
                    AccountNumber = string.Empty,
                    DocumentNumber = string.Empty,
                    PartialPaymentAmount = null,
                    PaymentAmount = string.Empty,
                    PaymentDate = null,
                    PaymentType = string.Empty
                });
            }

            return paymentSheets;
        }
    }
}
