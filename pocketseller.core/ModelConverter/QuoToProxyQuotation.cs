using System.Collections.Generic;

namespace pocketseller.core.ModelConverter
{
    public class QuoToProxyQuotation
    {
        public static ModelsAPI.Quotation CreateQuotation(Models.Quotation objDocument)
        {
            var objQuotation = new ModelsAPI.Quotation
            {
                Id = objDocument.Id,
                QuotationNr = objDocument.QuotationNr,
                StartDateTime = objDocument.StartDateTime,
                StopDateTime = objDocument.StopDateTime,
                State = objDocument.State,
                Phase = objDocument.Phase
            };

            var cobjQuotationdetails = new List<ModelsAPI.Quotationdetail>();

            foreach (var objDocumentdetail in objDocument.Quotationdetails)
            {
                var iIndex = new ModelsAPI.Quotationdetail
                {
                    Id = objDocumentdetail.Id,
                    QuotationId = objDocument.Id,
                    QuotationNr = objDocumentdetail.QuotationNr,
                    Pos = objDocumentdetail.Pos,
                    ArticleNr = objDocumentdetail.ArticleNr,
                    Count = objDocumentdetail.Count,
                    Content = objDocumentdetail.Content,
                    Amount = objDocumentdetail.Amount,
                    Nettoprice = objDocumentdetail.Nettoprice,
                    Nettosum = objDocumentdetail.Nettosum,
                    State = objDocumentdetail.State
                };
                cobjQuotationdetails.Add(iIndex);
            }

            objQuotation.Quotationdetails = cobjQuotationdetails;

            return objQuotation;
        }
    }

}
