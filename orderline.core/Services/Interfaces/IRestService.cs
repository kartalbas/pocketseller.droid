using orderline.core.ModelsAPI;
using orderline.core.ModelsPS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace pocketseller.core.Services.Interfaces
{
    public interface IRestService
    {
        Task<IList<MailResponse>> GetAllMails();
        Task<bool> DeleteMail(string messageId);
        Task<bool> SendMail(string from, string to, string subject, string body);

        Task<ObservableCollection<EMails>> GetMails();

        Task<FacturaData> GetFacturaData(int orderNumber);
        Task<IList<Stock>> GetAllStocks();

        Task<bool> Test();
        Task<string> GetMobile(string username, string password, string sourcename);
        Task<string> GetMobileToken(string username, string mobile, string token, string sourcename);
    }
}