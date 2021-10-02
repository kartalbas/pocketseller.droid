
namespace pocketseller.core.Services.Interfaces
{
    public interface IPrintService
    {
        void PrintNative(string strFile);

        void PrintWithHpApp(string strFile);
    }
}
