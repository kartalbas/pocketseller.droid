using orderline.core.ModelsAPI;
using orderline.core.ModelsPS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace pocketseller.core.Services.Interfaces
{
    public interface IRestService
    {
        Task<IList<MailResponse>> GetAllMailsAsync();
        Task<bool> DeleteMailAsync(string messageId);
        Task<bool> SendMailAsync(string from, string to, string subject, string body);

        Task<ObservableCollection<EMails>> GetMailsAsync();
        Task<ObservableCollection<Source>> GetSourcesAsync();

        Task<FacturaData> GetFacturaDataAsync(int orderNumber);

        Task<bool> Test();
        Task<string> GetMobile(string username, string password, string sourcename);
        Task<string> GetMobileToken(string username, string mobile, string token, string sourcename);
    }
}