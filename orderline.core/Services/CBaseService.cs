using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json.Linq;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.Services
{
    public class CBaseService : IBaseService
    {
        public string LogTag { get; set; }
        public object Lock { get; set; }
        public IMvxMessenger Messenger { get; set; }

        public CBaseService(IMvxMessenger objMessenger)
        {
            Lock = new object();
            Messenger = objMessenger;
        }

        public string GetErrorMessage(string json)
        {
            var error = JObject.Parse(json);
            var message = error?.Value<string>("type");
            return message ?? "No Message found";
        }

    }
}
