using orderline.core.ModelsPS;
using SQLite;
using System;
using System.Collections.Generic;

namespace pocketseller.core.ModelsAPI
{
    public class Order
    {
        public int DocumentType { get; set; }
        public Guid Id { get; set; }
        public string User { get; set; }
        public int Docnumber { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Info { get; set; }
        public decimal TotalParcels { get; set; }
        public decimal TotalNetto { get; set; }
        public decimal TotalBrutto { get; set; }
        public string Adressnumber { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Localtaxnr { get; set; }
        public string Eurtaxnr { get; set; }
        public int State { get; set; }
        public int Phase { get; set; }
        public string Response { get; set; }
        public string Usr1 { get; set; }
        public string Usr2 { get; set; }
        public string Usr3 { get; set; }
        public string Usr4 { get; set; }
        public string Usr5 { get; set; }
        public string Usr6 { get; set; }
        public string Usr7 { get; set; }
        public string Usr8 { get; set; }
        public string Usr9 { get; set; }
        public string Usr10 { get; set; }
        public string Usr11 { get; set; }
        public string Usr12 { get; set; }
        public string Usr13 { get; set; }
        public string Usr14 { get; set; }
        public string Usr15 { get; set; }
        public string Usr16 { get; set; }
        public string Usr17 { get; set; }
        public string Usr18 { get; set; }
        public string Usr19 { get; set; }

        public decimal Vat => TotalBrutto - TotalNetto;

        public virtual List<Orderdetail> Orderdetails { get; set; }

        [Ignore]
        public FacturaData FacturaData { get; set; }
    }
}
