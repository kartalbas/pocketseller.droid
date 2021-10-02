using System;
using System.Collections.ObjectModel;
using System.Linq;
using pocketseller.core.ModelsAPI;
using SQLite;

namespace pocketseller.core.Models
{
	public class Quotationdetail : BaseModel
	{
        public Quotationdetail()
        {
	        LogTag = GetType().Name;
            Selected = false;
	    }

        private Guid _quotationId;
        private int _quotationNr;
        private string _articleNr;
		private decimal _nettoprice;
        private int _state;
        private int _pos;
        private Article _article;

        private decimal _count;
        private decimal _content;
        private decimal _amount;
        private decimal _nettosum;
        private decimal _bruttosum;

        [Indexed]
        public int QuotationNr { get => _quotationNr;
            set { _quotationNr = value; RaisePropertyChanged(() => QuotationNr); } }
        public Guid QuotationId
        {
            get => _quotationId;
            set { _quotationId = value; RaisePropertyChanged(() => QuotationId); }
        }
        [Indexed]
        public int Pos { get => _pos;
            set { _pos = value; RaisePropertyChanged(() => Pos); } }
        public string ArticleNr { get => _articleNr;
            set { _articleNr = value; RaisePropertyChanged(() => ArticleNr); } }
        public decimal Nettoprice { get => _nettoprice;
            set { _nettoprice = value; OnNettopriceChanged(); RaisePropertyChanged(() => Nettoprice); } }
        public int State { get => _state;
            set { _state = value; RaisePropertyChanged(() => State); } }
        public decimal Count { get => _count;
            set { _count = value; OnCountChanged(); RaisePropertyChanged(() => Count); } }
        public decimal Content { get => _content;
            set { _content = value; RaisePropertyChanged(() => Content); } }
        public decimal Amount { get => _amount;
            set { _amount = value; OnAmountChanged(); RaisePropertyChanged(() => Amount); } }
        public decimal Nettosum { get => _nettosum;
            set { _nettosum = value; RaisePropertyChanged(() => Nettosum); } }

        [Indexed]
        public decimal Bruttosum { get => _bruttosum;
            set { _bruttosum = value; RaisePropertyChanged(() => Bruttosum); } }

        [Ignore]
        public bool Selected { get; set; }

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
                    Nettosum = Amount * Nettoprice;
                    Bruttosum = (1 + (Article.Tax / 100)) * Nettosum;
                }
            }
        }

        [Ignore]
        public Article Article
        {
            get
            {
                if (_article == null && !string.IsNullOrEmpty(ArticleNr) && ArticleNr.Length>1)
                    _article = Article.FindByArticleNr(ArticleNr);
                return _article;
            }
            set
            {
                _article = value;
                ArticleNr = (_article != null) ? _article.Articlenumber : null;
                Content = (_article != null) ? _article.Content : 0;
                Nettoprice = (_article != null && _article.Articleprice != null) ? _article.Articleprice.Price : 0;
                if (State == (int)EOrderdetailState.NEW)
                    Count = 1;
                RaisePropertyChanged(() => Article);
            }
        }

        public static Quotationdetail FindById(Guid id)
        {
            var result = Table<Quotationdetail>().FirstOrDefault(a => a.Id == id);
            return result;
        }

        public static ObservableCollection<Quotationdetail> Find(Quotation objQuotation)
	    {
            if(objQuotation == null)
                return new ObservableCollection<Quotationdetail>();

            var cobjQuotationdetails = Table<Quotationdetail>()
                .Where<Quotationdetail>(a => a.QuotationNr == objQuotation.QuotationNr)
                .OrderBy(a => a.Pos);

            return new ObservableCollection<Quotationdetail>(cobjQuotationdetails);
	    }
    }
}
