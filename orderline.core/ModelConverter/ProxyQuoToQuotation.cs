using System.Collections.ObjectModel;

namespace pocketseller.core.ModelConverter
{
    public class ProxyQuoToQuotation
    {
        public static Models.Quotation CreateQuotation(ModelsAPI.Quotation document)
        {
            var quotation = new Models.Quotation
            {
                Id = document.Id,
                QuotationNr = document.QuotationNr,
                StartDateTime = document.StartDateTime,
                StopDateTime = document.StopDateTime,
                State = document.State,
                Phase = document.Phase
            };

            var quotationdetails = new ObservableCollection<Models.Quotationdetail>();

            foreach (var documentdetail in document.Quotationdetails)
            {
                quotationdetails.Add(CreateQuotationdetail(documentdetail));
            }

            quotation.Quotationdetails = quotationdetails;

            return quotation;
        }

        public static Models.Quotationdetail CreateQuotationdetail(ModelsAPI.Quotationdetail quotationdetail)
        {
            var newQuotationdetail = new Models.Quotationdetail
            {
                Id = quotationdetail.Id,
                QuotationId = quotationdetail.QuotationId,
                QuotationNr = quotationdetail.QuotationNr,
                Pos = quotationdetail.Pos,
                ArticleNr = quotationdetail.ArticleNr,
                Count = quotationdetail.Count,
                Content = quotationdetail.Content,
                Amount = quotationdetail.Amount,
                Nettoprice = quotationdetail.Nettoprice,
                Nettosum = quotationdetail.Nettosum,
                State = quotationdetail.State
            };

            return newQuotationdetail;
        }
    }

}
