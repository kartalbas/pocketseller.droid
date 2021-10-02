using System;
using System.Collections.ObjectModel;
using System.Linq;
using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;
using Quotation = pocketseller.core.Models.Quotation;
using Quotationdetail = pocketseller.core.Models.Quotationdetail;
using pocketseller.core.Messages;

namespace pocketseller.core.Services
{
    public class CDocumentService : CBaseService, IDocumentService, IBaseService
    {
        #region Private properties
        #endregion

        #region Constructors

        public CDocumentService(IMvxMessenger objMessenger) : base(objMessenger)
        {
            LogTag = GetType().Name;
            Init();
        }

        public void Init()
        {
            Document = CreateNewDocument;
            Documentdetail = CreateNewDocumentdetail;
            Quotation = CreateNewQuotation;
            Quotationdetail = CreateNewQuotationdetail;
        }

        #endregion

        #region Private methods

        #endregion

        #region Public methods Order
        public void DeleteDocument()
        {
            Document.DeleteDocument(Document);
            Init();
        }
        public void DeleteDocument(Document objDocument)
        {
            if(objDocument != null)
                Document.DeleteDocument(objDocument);
            else
                Document.DeleteDocument(Document);

            Init();

            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }
        public void SaveDocument()
        {
            Document.SaveOrUpdate();
            Init();
            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }
        public void RemoveDocumentdetail()
        {
            Document.Remove(Documentdetail).ReOrderDocumentdetails();
            Document.ChangetState(EOrderState.CHANGED);
        }

        public void UpdateDocumentdetail()
        {
            Document.Update(Documentdetail);
            Document.ChangetState(EOrderState.CHANGED);
            Documentdetail = CreateNewDocumentdetail;
        }
        public void AddDocumentdetail()
        {
            int iMaxDocumentdetail = OrderSettings.Instance.MaxDocumentdetails;
            if (Document.Documentdetails.Count >= iMaxDocumentdetail)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(Language.MaxDocumentDetailsAchieved, Language.Attention, Language.Ok);
                return;
            }

            Document.Add(Documentdetail);
            Document.ChangetState(EOrderState.CHANGED);
            Documentdetail = CreateNewDocumentdetail;
        }
        public void AddOrUpdateDocumentdetail()
        {
            if (Documentdetail.State == (int)EOrderdetailState.NEW)
                AddDocumentdetail();
            else
                UpdateDocumentdetail();
        }
        public bool ArticleAlreadyExistsInDocument()
        {
            var objArticle = Document.FindArticle(Documentdetail.ArticleNr);
            return objArticle != null;
        }
        #endregion

        #region Public methods Quotation
        public void DeleteQuotation()
        {
            Quotation.DeleteQuotation(Quotation);
            Init();
        }
        public void DeleteQuotation(Quotation objDocument)
        {
            if (objDocument != null)
                Quotation.DeleteQuotation(objDocument);
            else
                Quotation.DeleteQuotation(Quotation);

            Init();

            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }
        public void SaveQuotation()
        {
            Quotation.SaveOrUpdate();
            Init();
            Messenger.Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
        }
        public void RemoveQuotationdetail()
        {
            Quotation.Remove(Quotationdetail).ReOrderQuotationdetails();
            Quotation.ChangetState(EOrderState.CHANGED);
        }
        public void UpdateQuotationdetail()
        {
            Quotation.Update(Quotationdetail);
            Quotation.ChangetState(EOrderState.CHANGED);
            Quotationdetail = CreateNewQuotationdetail;
        }
        public void AddQuotationdetail()
        {
            Quotation.Add(Quotationdetail);
            Quotation.ChangetState(EOrderState.CHANGED);
            Quotationdetail = CreateNewQuotationdetail;
        }
        public void AddOrUpdateQuotationdetail()
        {
            if (Quotationdetail.State == (int)EOrderdetailState.NEW)
                AddQuotationdetail();
            else
                UpdateQuotationdetail();
        }
        public bool ArticleAlreadyExistsInQuotation()
        {
            var objArticle = Quotation.FindArticle(Quotationdetail.Article.Articlenumber);
            return objArticle != null;
        }
        public void CopyQuototationdetailsToOrderdetails()
        {
            foreach (var objQuotationdetail in Quotation.Quotationdetails)
            {
                if (objQuotationdetail.Selected)
                {
                    Documentdetail = new Documentdetail
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = Document.Id,
                        ArticleNr = objQuotationdetail.Article.Articlenumber,
                        Article = objQuotationdetail.Article,
                        Pos = 0,
                        Count = objQuotationdetail.Count,
                        Content = objQuotationdetail.Content,
                        Amount = objQuotationdetail.Amount,
                        Nettoprice = objQuotationdetail.Nettoprice,
                        Nettosum = objQuotationdetail.Nettosum,
                        State = (int)EOrderdetailState.NEW,
                        TimeStamp = DateTime.Now
                    };

                    AddDocumentdetail();
                }
            }
        }
        #endregion

        #region Public methods generic
        public void DiscarDocument()
        {
            Init();
        }

        public EMode Mode { get; set; }

        public decimal GetLocalAvailableCount(Article objArticle)
        {
            var result = objArticle.StockPerPackage;
            return result;
        }

        public decimal GetLocalAvailableAmount(Article objArticle)
        {
            var result = objArticle.StockPerAmount;
            return result;
        }
        #endregion

        #region Public properties Quotation
        public Quotation Quotation { get; set; }
        public Quotationdetail Quotationdetail { get; set; }
        public Quotation CreateNewQuotation =>
            new Quotation
            {
                Id = Guid.NewGuid(),
                State = (int)EOrderState.ORDER,
                StartDateTime = default(DateTime),
                StopDateTime = default(DateTime),
                Quotationdetails = new ObservableCollection<Quotationdetail>()
            };

        public Quotationdetail CreateNewQuotationdetail =>
            new Quotationdetail
            {
                Id = Guid.NewGuid(),
                QuotationId = Quotation.Id,
                State = (int)EOrderdetailState.NEW
            };

        public void ChangeQuotationdetailState(EOrderdetailState enmDocumentdetailState)
        {
            Quotationdetail.State = (int)enmDocumentdetailState;
        }
        #endregion

        #region Public properties Order
        public Lastprice Lastprice => Lastprice.GetLastPrice(Document.Adress, Documentdetail.Article);
        public Document Document { get; set; }
        public Documentdetail Documentdetail { get; set; }

        public Order Order { get; set; }

        public Document CreateNewDocument =>
            new Document
            {
                Id = Guid.NewGuid(),
                State = (int)EOrderState.ORDER,
                Documentdetails = new ObservableCollection<Documentdetail>()
            };

        public Documentdetail CreateNewDocumentdetail =>
            new Documentdetail
            {
                Id = Guid.NewGuid(),
                DocumentId = Document.Id,
                Count =  1,
                State = (int)EOrderdetailState.NEW
            };

        public void ChangeDocumentdetailState(EOrderdetailState enmDocumentdetailState)
        {
            Documentdetail.State = (int)enmDocumentdetailState;
        }
        
        public decimal GetAlreadyInOrderPerPackage(Article article)
        {
            var countPackage = Document?.Documentdetails?.Where(d => d.ArticleNr.Equals(article.Articlenumber)).Sum(d => d.Count);
            return countPackage ?? 0.0M;
        }

        #endregion

        public enum EMode
        {
            Order = 0,
            Quotation = 1
        }
    }
}
