using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace pocketseller.core.Models
{
    public class Articleprice : BaseModel
    {
        public Articleprice()
        {
            LogTag = GetType().Name;
        }

        [Indexed]
        public string Articlenumber  { get; set; }
		[Indexed]
        public int PriceGroupNr { get; set; }
        public decimal FromPiece { get; set; }
        public decimal Price { get; set; }
        public DateTime LastChange { get; set; }

        public static Articleprice FindById(string articleNr)
        {
            return
                (DataService.PocketsellerConnection.Table<Articleprice>().Where(a => a.Articlenumber == articleNr)).FirstOrDefault();
        }

        public static Articleprice GetArticleprice(Article objArticle)
        {
            var iAutopriceIndex = OrderSettings.Instance.AutoPrice;

            var objArticleprices = GetArticleprices(objArticle);

            if (objArticleprices != null && objArticleprices.Count >= iAutopriceIndex + 1)
                return objArticleprices[iAutopriceIndex];

            return EmptyArticleprice(objArticle.Articlenumber, iAutopriceIndex);
        }

        public static List<Articleprice> GetArticleprices(Article objArticle)
        {
            if (objArticle == null)
                return EmptyArticleprices;

            var objPriceList = DataService.PocketsellerConnection.Table<Articleprice>()
                .Where(ap => ap.Articlenumber == objArticle.Articlenumber)
                .OrderBy(ap => ap.PriceGroupNr)
                .ToList();

            var result = objPriceList.OrderBy(p => p.PriceGroupNr).ToList();
            if(result.Count == 0)
            {
                result = new List<Articleprice>
                {
                    new Articleprice {Articlenumber = objArticle.Articlenumber, FromPiece = 1, Price = 0, PriceGroupNr = 1},
                    new Articleprice {Articlenumber = objArticle.Articlenumber, FromPiece = 1, Price = 0, PriceGroupNr = 2},
                    new Articleprice {Articlenumber = objArticle.Articlenumber, FromPiece = 1, Price = 0, PriceGroupNr = 3}
                };
            }
            return result;
        }

        public static Articleprice EmptyArticleprice(string articleNr, int priceGroupNr)
        {
            return new Articleprice { Articlenumber = articleNr, FromPiece = 1, Price = 0, PriceGroupNr = priceGroupNr };
        }

        public static List<Articleprice> EmptyArticleprices => new List<Articleprice> { new Articleprice {Articlenumber = "", FromPiece = 1, Price = 0, PriceGroupNr = 1} };
    }
}
