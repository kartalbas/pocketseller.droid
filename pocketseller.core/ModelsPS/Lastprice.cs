using System;
using SQLite;

namespace pocketseller.core.Models
{
    public class Lastprice : BaseModel
	{
        public Lastprice()
        {
            LogTag = GetType().Name;
        }

		[Indexed]
		public string Adressnumber  { get; set; }
		[Indexed]
		public string Articlenumber  { get; set; }
		public DateTime Salesdate  { get; set; }
		public decimal Count  { get; set; }
		public decimal Content  { get; set; }
		public decimal Price  { get; set; }

        public static Lastprice GetLastPrice(Adress objAddress, Article objArticle)
        {
            Lastprice objLastprice = null;

            if (objArticle != null && objAddress != null)
            {
                objLastprice = (DataService.PocketsellerConnection.Table<Lastprice>()
                                .Where(p => p.Adressnumber == objAddress.Adressnumber && p.Articlenumber == objArticle.Articlenumber))
                                .FirstOrDefault();                
            }

            if (objLastprice == null)
            {
                objLastprice = new Lastprice
                {
                    Adressnumber = objAddress != null ? objAddress.Adressnumber : "",
                    Articlenumber = objArticle != null ? objArticle.Articlenumber : "",
                    Content = objArticle != null ? objArticle.Content : 0,
                    Count = 0,
                    Price = 0,
                    Salesdate = default(DateTime)
                };                
            }

            return objLastprice;
        }

    }
}
