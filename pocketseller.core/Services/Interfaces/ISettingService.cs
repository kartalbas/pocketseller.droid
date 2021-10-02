
namespace pocketseller.core.Services.Interfaces
{
    public interface ISettingService
    {
        T Get<T>(ESettingType enmSetting);
        void Set<T>(ESettingType enmSetting, T objValue);
    }
}
