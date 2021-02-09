using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvvmCross;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using SQLite;

namespace pocketseller.core.Models
{
    public class Article : BaseModel
    {
        public Article()
        {
            LogTag = GetType().Name;
            if (DocumentService == null)
                DocumentService = (CDocumentService)Mvx.IoCProvider.Resolve<IDocumentService>();
        }

        [Collation("NOCASE")]
        [Indexed]
        public string Articlenumber { get; set; }

        [Collation("NOCASE")]
        [Indexed]
        public string Barcode { get; set; }

        [Collation("NOCASE")]
        [Indexed]
        public string Name1 { get; set; }

        [Collation("NOCASE")]
        [Indexed]
        public string Name2 { get; set; }

        [Indexed]
        public string Matchcode { get; set; }

        public string Info { get; set; }

        public string GroupTop { get; set; }

        public string GroupUnder { get; set; }

        public decimal Tax { get; set; }

        public decimal Content { get; set; }

        public decimal? MinimumPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? WeightPerKolli { get; set; }

        public string StockName { get; set; }

        public string StockPlace { get; set; }

        public int? StockAmount { get; set; }

        public DateTime? LastChange { get; set; }


        [Ignore]
        public decimal StockPerPackage => decimal.Round((StockAmount == null ? 0 : StockAmount.Value) / Content, 0, MidpointRounding.AwayFromZero);

        [Ignore]
        public decimal StockPerAmount => Convert.ToDecimal(StockAmount ?? 0);

        [Ignore]
        public decimal StockPerPackageLocal => StockPerPackage;

        [Ignore]
        public decimal StockPerAmountLocal => StockPerAmount;

        [Ignore]
        public decimal AlreadyInOrderPerPackage => DocumentService.GetAlreadyInOrderPerPackage(this);

        [Ignore]
        public List<Articleprice> Articleprices => Articleprice.GetArticleprices(this);

        public Articleprice ArticlepricesAt(int pos)
        {
            var prices = Articleprice.GetArticleprices(this);

            var result = prices.Count >= pos 
                ? prices[pos] 
                : new Articleprice { Articlenumber = "", FromPiece = 1, Price = 0, PriceGroupNr = pos + 1 };

            return result;
        }

        [Ignore]
        public Articleprice Articleprice => Articleprice.GetArticleprice(this);

        [Ignore]
        public string ThumbnailUrl => ImageUrl(50);

        [Ignore]
        public string ImageUrlDefaultThumbnail => ImageUrl(50, "default");

        [Ignore]
        public string ImageUrlErrorThumbnail => ImageUrl(50, "error");

        public string ImageUrl(int iWidth, string strPictureName = "")
        {
            if (strPictureName == "")
                strPictureName = Articlenumber;

            var strUrl = string.Empty;

            if (Uri.TryCreate(OrderSettings.Instance.PictureUrl, UriKind.Absolute, out _))
                strUrl = $@"{OrderSettings.Instance.PictureUrl}/{strPictureName}.{"jpg"}?{"width=" + iWidth}";

            return strUrl;
        }

        public static Article FindById(string articleNr)
        {
            return Table<Article>().Where(a => a.Articlenumber.Equals(articleNr)).FirstOrDefault();
        }

        public static Article FindByArticleNr(string iArticleNr)
        {
            string strArticleNr = iArticleNr.ToString();
            return (Table<Article>()
                .Where(a => a.Articlenumber.Equals(strArticleNr)))
                .FirstOrDefault();
        }

        public static decimal GetBruttoPrice(decimal nettoPrice, decimal taxInPercent)
        {
            var result = Math.Round(nettoPrice * (1 + (taxInPercent / 100)), 2, MidpointRounding.AwayFromZero);
            return result;
        }

        public static Tuple<decimal, decimal> GetTaxes()
        {
            var result = new Tuple<decimal, decimal>(0, 0);
            var taxes = Table<Article>().Select(a => a.Tax).Distinct().ToList();

            if(taxes.Count() == 2)
            {
                var tax0 = taxes.ElementAt(0);
                var tax1 = taxes.ElementAt(1);

                var restax0 = tax0 > tax1 ? tax0 : tax1;
                var restax1 = tax0 > tax1 ? tax1 : tax0;

                result = new Tuple<decimal, decimal>(restax0, restax1);
            }

            return result;
        }

        public static List<Article> Find(string strKey)
        {
            strKey = strKey.ToUpper();
            var enmGenerlSearchType = (EGeneralSearchType)SettingService.Get<int>(ESettingType.SearchType);
            var enmWordSearchType = (EWordSearchType)SettingService.Get<int>(ESettingType.SearchTypeAddress);

            switch (enmGenerlSearchType)
            {
                case EGeneralSearchType.Normal:
                    break;
                case EGeneralSearchType.LowerCase:
                    strKey = strKey.ToLower();
                    break;
                case EGeneralSearchType.UpperCase:
                    strKey = strKey.ToUpper();
                    break;
            }

            strKey = strKey.ToUpper(CultureInfo.InvariantCulture);

            switch (enmWordSearchType)
            {
                case EWordSearchType.BeginOfWord:
                    return
                        (Table<Article>()
                        .Where(a => a.Matchcode.ToUpper().StartsWith(strKey) || a.Barcode == strKey || a.Articlenumber.StartsWith(strKey))
                        .OrderBy(a => a.Articlenumber)).OrderBy(a => a.Name1)
                        .ToList();
                case EWordSearchType.OverAllWord:
                    return
                        (Table<Article>()
                        .Where(a => a.Matchcode.ToUpper().Contains(strKey) || a.Barcode == strKey || a.Articlenumber.Contains(strKey))
                        .OrderBy(a => a.Articlenumber)).OrderBy(a => a.Name1)
                        .ToList();
            }

            return new List<Article>();
        }
    }
}
