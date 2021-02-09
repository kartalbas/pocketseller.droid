using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCross;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ModelConverter
{
    public class Converter
    {
        public static Order CreateOrder(Source source, Document document)
        {
            var dataService = ((CDataService)Mvx.IoCProvider.Resolve<IDataService>());

            var order = new Order
            {
                Id = document.Id,
                DocumentType = document.Doctype,
                User = source.Username,
                Adressnumber = document.Adress.Adressnumber,
                Name1 = document.Adress.Name1,
                Name2 = document.Adress.Name2,
                Street = document.Adress.Street,
                Zip = document.Adress.Zip,
                City = document.Adress.City,
                Phone1 = document.Adress.Phone1,
                Phone2 = document.Adress.Phone2,
                Mobile = document.Adress.Mobile,
                Fax = document.Adress.Fax,
                Eurtaxnr = document.Adress.EurtaxNr,
                Localtaxnr = document.Adress.LocaltaxNr,
                TimeStamp = document.TimeStamp,
                Docnumber = document.Docnumber,
                Info = document.Info,
                TotalParcels = document.TotalParcels,
                TotalBrutto = document.TotalBrutto,
                TotalNetto = document.TotalNetto,
                State = document.State,
                Phase = document.Phase,
                Usr1 = string.Empty,
                Usr2 = string.Empty,
                Usr3 = string.Empty,
                Usr4 = string.Empty,
                Usr5 = string.Empty,
                Usr6 = string.Empty,
                Usr7 = string.Empty,
                Usr8 = string.Empty,
                Usr9 = string.Empty,
                Usr10 = string.Empty,
                Usr11 = string.Empty,
                Usr12 = string.Empty,
                Usr13 = string.Empty,
                Usr14 = string.Empty,
                Usr15 = string.Empty,
                Usr16 = string.Empty,
                Usr17 = string.Empty,
                Usr18 = string.Empty,
                Usr19 = source.UserId.ToString()
            };

            var orderdetails = new List<Orderdetail>();

            foreach (var documentdetail in document.Documentdetails)
            {
                var article = dataService.FindWithQuery<Article>($"SELECT * FROM Article WHERE Articlenumber = '{documentdetail.ArticleNr}'");
                var iIndex = new Orderdetail
                {
                    Id = documentdetail.Id,
                    OrderId = document.Id,
                    Pos =  documentdetail.Pos,
                    Articlenumber = documentdetail.Article.Articlenumber,
                    Barcode = documentdetail.Article.Barcode,
                    Name1 = documentdetail.Article.Name1,
                    Name2 = documentdetail.Article.Name2,
                    TaxInPercent = documentdetail.Article.Tax,
                    Docnumber = document.Docnumber,
                    Count = documentdetail.Count,
                    Content = documentdetail.Content,
                    Amount = documentdetail.Amount,
                    Nettoprice = documentdetail.Nettoprice,
                    Nettosum = documentdetail.Nettosum,
                    usr1 = article?.StockPlace
                };
                orderdetails.Add(iIndex);
            }

            order.Orderdetails = orderdetails;

            return order;
        }

        public static Document CreateDocument(Order order)
        {
            var dataService = ((CDataService)Mvx.IoCProvider.Resolve<IDataService>());

            var adress = dataService.FindWithQuery<Adress>($"SELECT * FROM Adress WHERE Adressnumber = '{order.Adressnumber}'");

            var document = new Document
            {
                Id = order.Id,
                Docnumber = order.Docnumber,
                AdressNr = adress.Adressnumber,
                TotalParcels = order.TotalParcels,
                TotalBrutto = order.TotalBrutto,
                TotalNetto = order.TotalNetto,
                TotalPos = order.Orderdetails.Count,
                Info = order.Info,
                EditMode = false,
                Doctype = (int)order.DocumentType,
                State = (int)EOrderState.ORDER,
                Phase = (int)EPhaseState.ACTIVATED,
                Adress = adress,
                TimeStamp = order.TimeStamp
            };

            var documentdetails = new List<Documentdetail>();

            foreach (var orderdetails in order.Orderdetails)
            {
                var article = dataService.FindWithQuery<Article>($"SELECT * FROM Article WHERE Articlenumber = '{orderdetails.Articlenumber}'");

                var documentdetail = new Documentdetail();
                documentdetail.Article = article;

                documentdetail.Id = orderdetails.Id;
                documentdetail.DocumentId = document.Id;
                documentdetail.State = (int)EOrderdetailState.EDIT;
                documentdetail.Pos = orderdetails.Pos;
                documentdetail.ArticleNr = orderdetails.Articlenumber;
                documentdetail.Count = orderdetails.Count;
                documentdetail.Content = orderdetails.Content;
                documentdetail.Amount = orderdetails.Amount;
                documentdetail.Nettoprice = orderdetails.Nettoprice;
                documentdetail.Nettosum = orderdetails.Nettosum;
                documentdetail.Bruttosum = (1 + (article.Tax / 100)) * orderdetails.Nettosum;
                documentdetail.TimeStamp = order.TimeStamp;
                documentdetail.State = (int)EOrderdetailState.NEW;
                documentdetails.Add(documentdetail);
            }

            document.Documentdetails = new ObservableCollection<Documentdetail>(documentdetails.OrderBy(o => o.Pos));

            return document;
        }

    }

}
