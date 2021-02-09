using System;
using System.Collections.ObjectModel;
using System.Linq;
using pocketseller.core.ModelsAPI;
using SQLite;

namespace pocketseller.core.Models
{
    public class Documentdetail : BaseModel
	{
	    public Documentdetail()
	    {
	        LogTag = GetType().Name;
            ArticleNr = "0";
	    }

	    private Guid _documentId;
        private string _articleNr;
		private decimal _count;
		private decimal _content;
		private decimal _amount;
		private decimal _nettoprice;
		private decimal _nettosum;
		private int _state;
		private int _pos;
		private decimal _bruttosum;
	    private Article _article;

	    [Indexed]
        [Collation("NOCASE")]
        public Guid DocumentId { get => _documentId;
	        set 
            { 
                _documentId = value; 
                RaisePropertyChanged(() => DocumentId); 
            } }
        [Indexed]
        public string ArticleNr { get => _articleNr;
	        set { _articleNr = value; RaisePropertyChanged(() => ArticleNr); } }
        public decimal Count { get => _count;
	        set { _count = value; OnCountChanged(); RaisePropertyChanged(() => Count); } }
		public decimal Content { get => _content;
			set { _content = value; RaisePropertyChanged(() => Content); } }
        public decimal Amount { get => _amount;
	        set { _amount = value; OnAmountChanged(); RaisePropertyChanged(() => Amount); } }
        public decimal Nettoprice { get => _nettoprice;
	        set { _nettoprice = value; OnNettopriceChanged(); RaisePropertyChanged(() => Nettoprice); } }
        public decimal Nettosum { get => _nettosum;
	        set { _nettosum = value; RaisePropertyChanged(() => Nettosum); } }
        public int Pos { get => _pos;
	        set { _pos = value; RaisePropertyChanged(() => Pos); } }
		public int State { get => _state;
			set { _state = value; RaisePropertyChanged(() => State); } }
		[Ignore]
		public decimal Bruttosum { get => _bruttosum;
			set { _bruttosum = value; RaisePropertyChanged(() => Bruttosum); } }

	    [Ignore]
	    public Article Article
	    {
	        get
            {
                if (_article == null && int.Parse(ArticleNr) > 0)
                    _article = Article.FindById(ArticleNr);
                return _article;
            }
	        set
	        {
	            _article = value;
                ArticleNr = (_article != null) ? _article.Articlenumber : "0";
                Content = (_article != null) ? _article.Content : 0;
	            Nettoprice = (_article != null && _article.Articleprice != null) ? _article.Articleprice.Price : 0;
                if(State == (int)EOrderdetailState.NEW)
                    Count = 1;
                RaisePropertyChanged(() => Article);
            }
	    }

        public void OnCountChanged()
        {
            lock (Lock)
            {
                Amount = Count * Content;
                OnDocumentChanged();
            }
        }

        public void OnAmountChanged()
        {
            OnDocumentChanged();
        }

        public void OnNettopriceChanged()
        {
            OnDocumentChanged();
        }

        public void OnDocumentChanged()
		{
            lock (Lock)
            {
                if (Article != null)
                {
                    Nettosum = Math.Round(Amount * Nettoprice, 4, MidpointRounding.AwayFromZero);
                    Bruttosum = Math.Round((1 + (Article.Tax / 100)) * Nettosum, 4, MidpointRounding.AwayFromZero);
                }
            }
		}

        public static Documentdetail FindById(Guid iId)
        {
            var result = DataService.PocketsellerConnection.Table<Documentdetail>().Where(a => a.Id == iId).FirstOrDefault();
            return result;
        }

        public static ObservableCollection<Documentdetail>Find(Document objDocument)
	    {
            if(objDocument == null)
                return new ObservableCollection<Documentdetail>();

            var objDocumentCollections = DataService.PocketsellerConnection.Table<Documentdetail>()
                .Where<Documentdetail>(a => a.DocumentId == objDocument.Id)
                .OrderBy(a => a.Pos);

            return new ObservableCollection<Documentdetail>(objDocumentCollections);
	    }

	    public static decimal GetSavedCount(Article objArticle)
	    {
            var dSum = DataService.PocketsellerConnection.Table<Documentdetail>()
                .Where<Documentdetail>(d => d.Article.Id == objArticle.Id)
	            .Sum(d => d.Count);

            return dSum;
	    }

        public static decimal GetSavedAmount(Article objArticle)
        {
            var dSum = DataService.PocketsellerConnection.Table<Documentdetail>()
                .Where<Documentdetail>(d => d.Article.Id == objArticle.Id)
	            .Sum(d => d.Amount);

            return dSum;
        }
    }
}
