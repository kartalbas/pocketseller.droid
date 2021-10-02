using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using pocketseller.core.ModelsAPI;
using SQLite;

namespace pocketseller.core.Models
{
	public class Quotation : BaseModel
	{

        public Quotation()
        {
            LogTag = GetType().Name;
            EditMode = false;
        }

        private int _quotationNr;
        private DateTime _startDateTime;
        private DateTime _stopDateTime;
        private int _state;
        private int _phase;
        private ObservableCollection<Quotationdetail> _quotationdetails;
        private ObservableCollection<Quotationdetail> _quotationdetailsDeleted;

        [Indexed]
        public int QuotationNr { get => _quotationNr;
            set { _quotationNr = value; RaisePropertyChanged(() => QuotationNr); } }        
	    public DateTime StartDateTime { get => _startDateTime;
            set { _startDateTime = value; RaisePropertyChanged(() => StartDateTime); } }
	    public DateTime StopDateTime { get => _stopDateTime;
            set { _stopDateTime = value; RaisePropertyChanged(() => StopDateTime); } }
	    public int State { get => _state;
            set { _state = value; RaisePropertyChanged(() => State); } }
        public int Phase { get => _phase;
            set { _phase = value; RaisePropertyChanged(() => Phase); } }

        [Ignore]
        public bool EditMode { get; set; }
        [Ignore]
	    public string Response { get; set; }

        [Ignore]
        public ObservableCollection<Quotationdetail> Quotationdetails
	    {
	        get
	        {
                if (_quotationdetails == null)
                    _quotationdetails = Quotationdetail.Find(this);
                return _quotationdetails;
            }
	        set
	        {
	            _quotationdetails = value; 
                RaisePropertyChanged(() => Quotationdetails);
	        }
	    }

        [Ignore]
        public ObservableCollection<Quotationdetail> QuotationdetailsDeleted
        {
            get
            {
                if (_quotationdetailsDeleted == null)
                    _quotationdetailsDeleted = new ObservableCollection<Quotationdetail>();
                return _quotationdetailsDeleted;
            }
            set
            {
                _quotationdetailsDeleted = value;
                RaisePropertyChanged(() => _quotationdetailsDeleted);
            }
        }

        public void ReOrderQuotationdetails()
        {
            int iPos = 1;
            foreach (var objDocDetail in Quotationdetails)
                objDocDetail.Pos = iPos++;
        }

        public Article FindArticle(string articleNr)
        {
            var objDocumentdetail = Quotationdetails.FirstOrDefault(article => article.ArticleNr.Equals(articleNr));
            return objDocumentdetail != null ? objDocumentdetail.Article : null;
        }

        public Quotation Add(Quotationdetail objDocumentdetail)
        {
            objDocumentdetail.Pos = Quotationdetails.Count + 1;
            Quotationdetails.Add(objDocumentdetail);
            return this;
        }

        public Quotation Update(Quotationdetail objDocumentdetail)
        {
            int iIndex = Quotationdetails.IndexOf(objDocumentdetail);
            Remove(objDocumentdetail);
            Quotationdetails.Insert(iIndex, objDocumentdetail);
            return this;
        }

        public Quotation Remove(Quotationdetail objDocumentdetail)
        {
            Quotationdetails.Remove(objDocumentdetail);
            QuotationdetailsDeleted.Add(objDocumentdetail);
            return this;
        }

        public void ChangetState(EOrderState enmDocumentState)
        {
            State = (int)enmDocumentState;
        }
        public static void ChangePhase(Quotation objDoc, EPhaseState enmPhase)
        {
            if (objDoc != null)
            {
                objDoc.Phase = (int)enmPhase;
                DataService.Update(objDoc);
            }
        }

        public static void UpdateQuotationnumber(Quotation objDoc, int iDocumentnumber)
        {
            if (objDoc != null)
            {
                objDoc.QuotationNr = iDocumentnumber;
                DataService.Update(objDoc);

                foreach (var objQuotationdetail in objDoc.Quotationdetails)
                {
                    objQuotationdetail.QuotationNr = iDocumentnumber;
                    DataService.Update(objQuotationdetail);
                }
            }
        }

        public static ObservableCollection<Quotation> FindValid()
        {
            var cobjDocs = Table<Quotation>()
                .Where(o => DateTime.Now >= o.StartDateTime && DateTime.Now <= o.StopDateTime )
                .OrderByDescending(o => o.TimeStamp).AsEnumerable();

            var result = new ObservableCollection<Quotation>(cobjDocs);
            return result;
        }

        public static ObservableCollection<Quotation> FindPast()
        {
            IEnumerable<Quotation> cobjDocs = (Table<Quotation>()
                .Where(o => DateTime.Now > o.StartDateTime && DateTime.Now > o.StopDateTime)
                .OrderByDescending(o => o.TimeStamp)).AsEnumerable();

            return new ObservableCollection<Quotation>(cobjDocs);
        }

        public static ObservableCollection<Quotation> FindSent()
        {
            IEnumerable<Quotation> cobjDocs = (DataService.PocketsellerConnection.Table<Quotation>()
                .Where(o => o.Phase == (int)EPhaseState.SERVER)
                .OrderByDescending(o => o.TimeStamp)).AsEnumerable();

            return new ObservableCollection<Quotation>(cobjDocs);
        }

        public static ObservableCollection<Quotation> FindNewOrChanged()
        {
            IEnumerable<Quotation> cobjDocs = (DataService.PocketsellerConnection.Table<Quotation>()
                .Where(o => o.Phase == (int)EPhaseState.LOCAL || o.Phase == (int)EPhaseState.ACTIVATED)
                .OrderByDescending(o => o.TimeStamp)).AsEnumerable();

            return new ObservableCollection<Quotation>(cobjDocs);
        }

        public static void DeleteQuotations(EPhaseState enmState)
        {
            foreach (Quotation objDocument in GetAllQuotations(enmState))
                DeleteQuotation(objDocument);
        }

        public static ObservableCollection<Quotation> GetAllQuotations(EPhaseState enmState)
        {
            return new ObservableCollection<Quotation>(Table<Quotation>().Where(a => a.Phase == (int)enmState).OrderBy(a => a.TimeStamp));
        }
        
        public static void DeleteQuotation(Quotation objDocument)
        {
            try
            {
                DataService.PocketsellerConnection.BeginTransaction();

                if (objDocument != null)
                {
                    if (objDocument.Quotationdetails != null)
                    {
                        foreach (var objDocDetail in Quotationdetail.Find(objDocument))
                            DataService.Delete(objDocDetail);
                    }

                    DataService.Delete(objDocument);
                }

                DataService.PocketsellerConnection.Commit();
            }
            catch (Exception)
            {
                DataService.PocketsellerConnection.Rollback();
            }
        }

        public void SaveOrUpdate()
        {
            try
            {
                DataService.PocketsellerConnection.BeginTransaction();

                if (EditMode)
                    State = (int)EOrderState.CHANGED;
                else
                    State = (int)EOrderState.ORDER;

                DataService.InsertOrUpdate(this);

                //Delete first deleted Documentdetails
                foreach (var objDocDetail in QuotationdetailsDeleted)
                    DataService.Delete(objDocDetail);

                //Insert or Update Documentdetails
                foreach (var objDocDetail in Quotationdetails)
                {
                    objDocDetail.QuotationNr = QuotationNr;
                    DataService.InsertOrUpdate(objDocDetail);
                }

                DataService.PocketsellerConnection.Commit();
            }
            catch (Exception)
            {
                DataService.PocketsellerConnection.Rollback();
            }
        }
    }

}
