using System;
using System.IO;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Models;
using pocketseller.core.Services.Interfaces;
using SQLite;

namespace pocketseller.core.Services
{
    public class CDataService : CBaseService, IDataService, IBaseService
    {
        public SQLiteConnection PocketsellerConnection { get; set; }
        public SQLiteConnection SettingsConnection { get; set; }

        private string MyDocumentFolder { get; set; }

        public CDataService(IMvxMessenger objMessenger)
            : base(objMessenger)
        {
            LogTag = GetType().Name;
            var objFileSystem = Mvx.IoCProvider.Resolve<IBasicPlatformService>();
            MyDocumentFolder = objFileSystem.GetMyDocumentsFolder();
        }

        public void CreateSettingDb()
        {
            try
            {
                string strDatabase = Path.Combine(MyDocumentFolder, CSettingService.SettingsDb);
                SettingsConnection = new SQLiteConnection(strDatabase);

                SettingsConnection.CreateTable<Settings>();
                SettingsConnection.CreateTable<Source>();
                SettingsConnection.CreateTable<EMails>();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public void CreatePocketsellerDb(string strDatabase)
        {
            string strDatabaseFile = Path.Combine(MyDocumentFolder, strDatabase);

            if (PocketsellerConnection != null)
            {
                TransferInfo.Instance.Kill();
                OrderSettings.Instance.Kill();
                PocketsellerConnection.Close();
            }

            PocketsellerConnection = new SQLiteConnection(strDatabaseFile);
           
            PocketsellerConnection.CreateTable<TransferInfo>();
            PocketsellerConnection.CreateTable<OrderSettings>();
            PocketsellerConnection.CreateTable<Company>();
            PocketsellerConnection.CreateTable<Adress>();
            PocketsellerConnection.CreateTable<Article>();
            PocketsellerConnection.CreateTable<Articleprice>();
            PocketsellerConnection.CreateTable<Lastprice>();
            PocketsellerConnection.CreateTable<OpenPayment>();
            PocketsellerConnection.CreateTable<Document>();
            PocketsellerConnection.CreateTable<Documentdetail>();
            PocketsellerConnection.CreateTable<Quotation>();
            PocketsellerConnection.CreateTable<Quotationdetail>();
        }

        public void RecreatePocketsellerTables()
        {
            Recreate<TransferInfo>(PocketsellerConnection);
            Recreate<Company>(PocketsellerConnection);
            Recreate<Adress>(PocketsellerConnection);
            Recreate<Article>(PocketsellerConnection);
            Recreate<Articleprice>(PocketsellerConnection);
            Recreate<Lastprice>(PocketsellerConnection);
            Recreate<OpenPayment>(PocketsellerConnection);
        }

        public void RecreateSourceTables()
        {
            Recreate<Source>(SettingsConnection);
            Recreate<EMails>(SettingsConnection);
            Recreate<Company>(SettingsConnection);
        }

        public void InsertOrUpdate<T>(T objItem) where T : new()
        {
            lock (Lock)
            {
                var objObject = objItem as BaseModel;

                if (objObject != null && Find<T>(objObject.Id) != null)
                    Update(objItem);
                else
                    Insert(objItem);
            }
        }

        public void Recreate<T>(SQLiteConnection connection)
        {
            lock (Lock)
            {
                connection?.BeginTransaction();
                connection?.DropTable<T>();
                connection?.CreateTable<T>();
                connection?.Commit();
            }
        }

        public int Count<T>() where T : new()
        {
            lock (Lock)
            {
                return PocketsellerConnection.Table<T>().Count();
            }
        }

        public void Insert<T>(T objItem)
        {
            lock (Lock)
            {
                PocketsellerConnection.Insert(objItem);
            }
        }

        public void Update<T>(T objItem)
        {
            lock (Lock)
            {
                PocketsellerConnection.Update(objItem);
            }
        }

        public void Delete<T>(T objItem)
        {
            lock (Lock)
            {
                PocketsellerConnection.Delete(objItem);
            }
        }

        public T Find<T>(object objPrimaryKey) where T : new()
        {
            lock (Lock)
            {
                T objItem = PocketsellerConnection.Find<T>(objPrimaryKey);
                return objItem;
            }
        }

        public T FindWithQuery<T>(string query) where T : new()
        {
            lock (Lock)
            {
                T objItem = PocketsellerConnection.FindWithQuery<T>(query);
                return objItem;
            }
        }
    }
}
