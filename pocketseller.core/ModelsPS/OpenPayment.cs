using System;
using System.Collections.ObjectModel;
using System.Linq;
using SQLite;

namespace pocketseller.core.Models
{
    public class OpenPayment : BaseModel
	{
	    public OpenPayment()
	    {
	        LogTag = GetType().Name;
	    }

		[Indexed]
		public string Adressnumber  { get; set; }
		public string Docnumber  { get; set; }
		public DateTime? Docdate  { get; set; }
		public decimal Amountopen  { get; set; }
		public decimal Amountorder  { get; set; }

		public static void Delete(OpenPayment op)
		{
			if (op == null)
				return;

			var objResult = DataService.PocketsellerConnection.Table<OpenPayment>()
				.FirstOrDefault(a => (a.Docnumber == op.Docnumber && a.Docdate == op.Docdate));
			
			DataService.Delete(objResult);
		}
		
	    public static ObservableCollection<OpenPayment> Find(Adress objAddress)
	    {
	        if (objAddress == null)
	            return null;

            var objResult = DataService.PocketsellerConnection.Table<OpenPayment>()
                .Where(a => (a.Adressnumber == objAddress.Adressnumber))
                .OrderBy(a => a.Docdate).ToList();

            return new ObservableCollection<OpenPayment>(objResult);
	    }

        public static bool IsWithinPaymentDays(Adress adress)
        {
            if (adress == null)
                return true;

            int days = adress.PaymentDays;
                
            var opList = Find(adress);

            var positiveOpList = opList?.Where(a => a.Amountopen > 0)?.ToList();

            if (positiveOpList == null)
                return true;

            if (positiveOpList.Count == 0)
                return true;

            var orderdOpList = positiveOpList
                .OrderBy(a => a.Docdate)
                .ToList();

            var op = orderdOpList.ElementAt(0);

            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var maxPossibleDate = currentDate.AddDays(-1*days);
            var invoicable = op.Docdate > maxPossibleDate;

            return invoicable;
        }

        public static decimal GetTotalOpen(Adress objAddress)
	    {
            if (objAddress == null)
                return 0;

            var objResult = DataService.PocketsellerConnection.Table<OpenPayment>()
                .Where(a => a.Adressnumber == objAddress.Adressnumber)
                .Sum(a => a.Amountopen);

            return objResult;
        }        
	}
}
