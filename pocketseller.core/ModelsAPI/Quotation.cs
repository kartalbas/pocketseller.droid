using System;
using System.Collections.Generic;

namespace pocketseller.core.ModelsAPI
{
    public class Quotation
	{
        public Guid Id { get; set; }
        public int QuotationNr { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime StopDateTime { get; set; }
        public int State { get; set; }
        public int Phase { get; set; }

        public virtual List<Quotationdetail> Quotationdetails { get; set; }
    }
}
