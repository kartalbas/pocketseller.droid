using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Services.Interfaces
{
    public interface IBaseService
    {
        string LogTag { get; set; }
        
        object Lock { get; set; }
        
        IMvxMessenger Messenger { get; set; }

        string GetErrorMessage(string json);
    }
}
