
using System;
using System.Collections.Generic;
using System.Linq;

namespace pocketseller.core.ModelsAPI
{
    public class Orderdetail
    {
        public Guid Id { get; set; }
        public int Pos { get; set; }
        public Guid OrderId { get; set; }
        public int Docnumber { get; set; }
        public decimal Count { get; set; }
        public decimal Content { get; set; }
        public decimal Amount { get; set; }
        public decimal Nettoprice { get; set; }
        public decimal Nettosum { get; set; }
        public string Articlenumber { get; set; }
        public string Barcode { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public decimal TaxInPercent { get; set; }
        public string usr1 { get; set; }
        public string usr2 { get; set; }
        public string usr3 { get; set; }
        public string usr4 { get; set; }
        public string usr5 { get; set; }
        public string usr6 { get; set; }
        public string usr7 { get; set; }
        public string usr8 { get; set; }
        public string usr9 { get; set; }
        public string usr10 { get; set; }
        public string usr11 { get; set; }

        public static Tuple<decimal, decimal> GetNettoSum(List<Orderdetail> orderdetails, decimal tax0, decimal tax1)
        {
            var nettoAmount0 = Math.Round(orderdetails.Where(o => o.TaxInPercent == tax0).Sum(a => a.Nettosum), 2, MidpointRounding.AwayFromZero);
            var nettoAmount1 = Math.Round(orderdetails.Where(o => o.TaxInPercent == tax1).Sum(a => a.Nettosum), 2, MidpointRounding.AwayFromZero);
            var result = new Tuple<decimal, decimal>(nettoAmount0, nettoAmount1);
            return result;
        }

        private object Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
