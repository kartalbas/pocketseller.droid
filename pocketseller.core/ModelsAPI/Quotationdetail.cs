using System;

namespace pocketseller.core.ModelsAPI
{
    public class Quotationdetail
	{
        public Guid Id { get; set; }
        public Guid QuotationId { get; set; }
        public int QuotationNr { get; set; }
        public int Pos { get; set; }
        public string ArticleNr { get; set; }
        public decimal Nettoprice { get; set; }
        public int State { get; set; }
        public decimal Count { get; set; }
        public decimal Content { get; set; }
        public decimal Amount { get; set; }
        public decimal Nettosum { get; set; }
    }
}
