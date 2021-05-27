using orderline.core.ModelsPS;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace pocketseller.core.Services.Interfaces
{
    public interface IRestService
    {
        Task<ObservableCollection<EMails>> GetMailsAsync();
        Task<ObservableCollection<Source>> GetSourcesAsync();
        Task<FacturaData> GetFacturaDataAsync(int orderNumber);

        Task<string> GetToken();
    }
}