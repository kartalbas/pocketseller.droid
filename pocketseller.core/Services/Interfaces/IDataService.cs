
using SQLite;
using System.Collections.Generic;

namespace pocketseller.core.Services.Interfaces
{
    public interface IDataService
    {
        void Insert<T>(T objItem);
        void Update<T>(T objItem);
        void InsertOrUpdate<T>(T objItem) where T : new();
        void Delete<T>(T objItem);
        List<T> Query<T>(string query) where T : new();
        void Recreate<T>(SQLiteConnection connection);
        T Find<T>(object objPrimaryKey) where T : new();
        T FindWithQuery<T>(string query) where T : new();
        int Count<T>() where T : new();
    }
}
