using System;
using System.Collections.Generic;
using System.Globalization;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using SQLite;

namespace pocketseller.core.Models
{
    public class Adress : BaseModel
    {
        public Adress()
        {
            LogTag = GetType().Name;
        }

        [Collation("NOCASE")]
        [Indexed]
        public string Adressnumber { get; set; }
        [Collation("NOCASE")]
        [Indexed]
        public string Name1 { get; set; }
        [Collation("NOCASE")]
        [Indexed]
        public string Name2 { get; set; }
        [Indexed]
        public string Matchcode { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string PriceGroupNr { get; set; }
        public string GroupName { get; set; }
        public string Taxgroup { get; set; }
        public string Creditlimit { get; set; }
        public string LocaltaxNr { get; set; }
        public string EurtaxNr { get; set; }
        public int PaymentDays { get; set; }
        public DateTime? LastChange { get; set; }

        public static bool IsCustomerWithoutMwSt(string auslandsart)
        {
            var enumAuslandsart = Enum.Parse(typeof(Auslandsart), auslandsart ?? "0");
            var result = (Auslandsart)enumAuslandsart == Auslandsart.EgMitId || (Auslandsart)enumAuslandsart == Auslandsart.Ausland;
            return result;
        }

        public static Adress FindById(string adressNr)
        {
            var result = (DataService.PocketsellerConnection.Table<Adress>().Where(a => a.Adressnumber.Equals(adressNr))).FirstOrDefault();
            return result;
        }

        public static Adress FindByKontoNr(string kontoNr)
        {
            var result = (DataService.PocketsellerConnection.Table<Adress>().Where(a => a.Adressnumber.Equals(kontoNr))).FirstOrDefault();
            return result;
        }

        public static List<Adress> Find(string strKey)
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
            var result = new List<Adress>();

            switch (enmWordSearchType)
            {
                case EWordSearchType.BeginOfWord:
                    result = Table<Adress>()
                            .Where(a => a.Matchcode.ToUpper().StartsWith(strKey) || a.Adressnumber == strKey)
                            .OrderBy(a => a.Adressnumber).OrderBy(a => a.Name1).ToList();
                    return result;
                case EWordSearchType.OverAllWord:
                    result = Table<Adress>()
                            .Where(a => a.Matchcode.ToUpper().Contains(strKey) || a.Adressnumber == strKey)
                            .OrderBy(a => a.Adressnumber).OrderBy(a => a.Name1).ToList();
                    return result;
            }

            return result;
        }

        public static List<Adress> GetAll()
        {
            return Table<Adress>().ToList();
        }

        public static Adress EmptyAdress(string strEmptyChar = "*")
        {
            return new Adress { Adressnumber = strEmptyChar, Name1 = Language.NoResult, Name2 = strEmptyChar, City = strEmptyChar, Zip = strEmptyChar, Street = strEmptyChar };
        }

        private enum Auslandsart
        {
            Inland = 0,
            Ausland = 1,
            EgMitId = 2,
            EgOhneId = 3,
        }
    }
}
