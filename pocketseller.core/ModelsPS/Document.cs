using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using pocketseller.core.ModelsAPI;
using SQLite;

namespace pocketseller.core.Models
{
	public class Document : BaseModel
	{

	    public Document()
	    {
            LogTag = GetType().Name;
	        EditMode = false;
            LocalDocument = false;
	    }

		private int _doctype;
        private string _adressNr;
        private int _state;
        private int _phase;
        private int _docnumber;
        private string _info;
        private decimal _profit;
        private decimal _totalParcels;
        private decimal _totalNetto;
        private int _totalPos;
        private decimal _totalBrutto;
        private ObservableCollection<Documentdetail> _documentdetails;
        private ObservableCollection<Documentdetail> _documentdetailsDeleted;
        private Adress _address;

        [Indexed]
        [Collation("NOCASE")]
        public int Docnumber
        {
            get => _docnumber;
            set { _docnumber = value; RaisePropertyChanged(() => Docnumber); }
        }

        [Indexed]
        public string AdressNr
        {
            get => _adressNr;
	        set { _adressNr = value; RaisePropertyChanged(() => AdressNr); }
        }

        [Indexed]
        public int Doctype
        {
            get => _doctype;
	        set { _doctype = value; RaisePropertyChanged(() => Doctype); }
        }

        [Indexed]
        public int State
        {
            get => _state;
	        set { _state = value; RaisePropertyChanged(() => State); }
        }

        public decimal Profit
        {
            get => _profit;
            set { _profit = value; RaisePropertyChanged(() => Profit); }
        }

        public int Phase
        {
            get => _phase;
	        set { _phase = value; RaisePropertyChanged(() => Phase); }
        }

        public decimal TotalParcels
        {
            get => _totalParcels;
	        set { _totalParcels = value; RaisePropertyChanged(() => TotalParcels); }
        }

        public decimal TotalNetto 
        {
            get => _totalNetto;
	        set { _totalNetto = value; RaisePropertyChanged(() => TotalNetto); }
        }

        public decimal TotalBrutto
        {
            get => _totalBrutto;
	        set { _totalBrutto = value; RaisePropertyChanged(() => TotalBrutto); }
        }

        public int TotalPos
        {
            get => _totalPos;
	        set { _totalPos = value; RaisePropertyChanged(() => TotalPos); }
        }

        [Ignore]
        public bool LocalDocument { get; set; }

        public string Info
        {
            get => _info;
            set
            {
                if (_info == value)
                    return;

                _info = value;
                ChangetState(EOrderState.CHANGED);
                RaisePropertyChanged(() => Info);
            }
        }
	    public string Response { get; set; }

        [Ignore]
        public bool EditMode { get; set; }

        [Ignore]
	    public Adress Adress
	    {
	        get
	        {
                if (_address == null)
                    if (!string.IsNullOrEmpty(AdressNr) && AdressNr.Length > 1)
                        _address = Adress.FindById(AdressNr);
                return _address;
            }
	        set
	        {
	            _address = value;
                RaisePropertyChanged(() => Adress);
	            AdressNr = _address != null ? _address.Adressnumber : "0";
	        }
	    }

	    [Ignore]
        public ObservableCollection<Documentdetail> Documentdetails
	    {
	        get
	        {
                if (_documentdetails == null)
                    _documentdetails = Documentdetail.Find(this);
	            return _documentdetails;
	        }
	        set
	        {
	            _documentdetails = value;
                RaisePropertyChanged(() => Documentdetails); 
	        }
	    }

        [Ignore]
        public ObservableCollection<Documentdetail> DocumentdetailsDeleted
        {
            get
            {
                if (_documentdetailsDeleted == null)
                    _documentdetailsDeleted = new ObservableCollection<Documentdetail>();
                return _documentdetailsDeleted;
            }
            set
            {
                _documentdetails = value;
                RaisePropertyChanged(() => DocumentdetailsDeleted);
            }
        }

        public int GetNextOrdernumber()
        {
            var iCurrentDocNr = OrderSettings.Instance.CurrentDocNr + 1;
            OrderSettings.Instance.CurrentDocNr = iCurrentDocNr;
            return iCurrentDocNr;
        }

        private void CalculateTotals()
	    {
            TotalNetto = Documentdetails?.Sum(p => p.Nettosum) ?? 0;
            TotalBrutto = Documentdetails != null ? Math.Round(Documentdetails.Sum(p => p.Bruttosum), 2, MidpointRounding.AwayFromZero) : 0;
            TotalParcels = Documentdetails?.Sum(p => p.Count) ?? 0;
            Profit = Documentdetails?.Sum(p => p.Profit) ?? 0;
        }

        public static ObservableCollection<Document> FindSent()
		{
            var cobjDocs = (DataService.PocketsellerConnection.Table<Document>()
                .Where(o => o.Phase == (int)EPhaseState.SERVER)
                .OrderByDescending(o => o.TimeStamp)).AsEnumerable();

            return new ObservableCollection<Document>(cobjDocs);
		}

        public static ObservableCollection<Document> FindNewOrChanged()
        {
            var cobjDocs = (DataService.PocketsellerConnection.Table<Document>()
                .Where(o => o.Phase == (int)EPhaseState.LOCAL || o.Phase == (int)EPhaseState.ACTIVATED)
                .OrderByDescending(o => o.TimeStamp)).AsEnumerable();

            return new ObservableCollection<Document>(cobjDocs);
        }

        public static Document FindById(Guid id)
		{
            return (DataService.PocketsellerConnection.Table<Document>().Where(a => a.Id == id)).FirstOrDefault();
		}

        public static ObservableCollection<Document> GetAllDocuments(EOrderState enmState)
        {
            return new ObservableCollection<Document>(Table<Document>().Where(a => a.State == (int)enmState).OrderBy(a => a.TimeStamp));
        }
		
        public void ReOrderDocumentdetails()
        {
            var iPos = 1;
            foreach (var objDocDetail in Documentdetails)
                objDocDetail.Pos = iPos++;
        }

	    public Article FindArticle(string articleNr)
	    {
	        var objDocumentdetail = (Documentdetails.Where(article => article.ArticleNr == articleNr)).FirstOrDefault();
	        return objDocumentdetail?.Article;
	    }

        public Document Add(Documentdetail documentdetail)
        {
            TotalPos = Documentdetails.Count + 1;
            documentdetail.Pos = TotalPos;
            Documentdetails.Add(documentdetail);
            CalculateTotals();
            return this;
        }

        public Document Update(Documentdetail documentdetail)
        {
            var iIndex = Documentdetails.IndexOf(documentdetail);
            Remove(documentdetail);
            Documentdetails.Insert(iIndex, documentdetail);
            CalculateTotals();
            return this;
        }

	    public Document Remove(Documentdetail documentdetail)
	    {
	        Documentdetails.Remove(documentdetail);
            DocumentdetailsDeleted.Add(documentdetail);
            CalculateTotals();
            return this;
	    }

        public static void ChangeState(Document objDoc, EOrderState enmState)
		{
		    if (objDoc != null)
		    {
		        objDoc.State = (int)enmState;
		        DataService.Update(objDoc);
            }
		}

        public static void ChangePhase(Document objDoc, EPhaseState enmPhase)
        {
            if (objDoc != null)
            {
                objDoc.Phase = (int)enmPhase;
                DataService.Update(objDoc);
            }
        }

        public static void UpdateDocumentnumber(Document objDoc, int iDocumentnumber)
        {
            if (objDoc != null)
            {
                objDoc.Docnumber = iDocumentnumber;
                DataService.Update(objDoc);
            }
        }

        public void ChangetState(EOrderState enmDocumentState)
        {
            State = (int)enmDocumentState;
        }
        
        public void SaveOrUpdate()
	    {
	        try
	        {
                DataService.PocketsellerConnection.BeginTransaction();

                //Insert or Updat Document
	            if (Docnumber == 0)
	            {
                    Docnumber = GetNextOrdernumber();
                }

                Doctype = TotalBrutto > 0 ? (int)EDocumentType.ORDER : (int)EDocumentType.CREDITNOTE;

                if (EditMode)
                {
                    State = (int)EOrderState.CHANGED;
                }
                else
                {
                    State = (int)EOrderState.ORDER;
                }

                DataService.InsertOrUpdate(this);

                //Delete first deleted Documentdetails
                foreach (var objDocDetail in DocumentdetailsDeleted)
                    DataService.Delete(objDocDetail);

                //Insert or Update Documentdetails
	            foreach (var objDocDetail in Documentdetails)
	            {
                    objDocDetail.DocumentId = this.Id;
                    DataService.InsertOrUpdate(objDocDetail);
	            }

                DataService.PocketsellerConnection.Commit();
	        }
	        catch (Exception)
	        {
                DataService.PocketsellerConnection.Rollback();
	        }
	    }

        public static void DeleteDocuments(EOrderState enmState)
        {
            foreach (Document objDocument in GetAllDocuments(enmState))
                DeleteDocument(objDocument);
        }

        public static void DeleteDocument(Document objDocument)
        {
            try
            {
                DataService.PocketsellerConnection.BeginTransaction();

                if (objDocument != null)
                {
                    if (objDocument.Documentdetails != null)
                    {
                        foreach (var objDocDetail in Documentdetail.Find(objDocument))
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

	}

}
