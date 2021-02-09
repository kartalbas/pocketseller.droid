namespace pocketseller.core.Services.Interfaces
{
    public interface IBasicPlatformService
    {
        void Finish();
        void ShowKeyboard();
        void HideKeyboard();
        void CreateFolderIfNotExists(string strFolder);
        void SaveText(string filename, string text);
        string LoadText(string filename);
        string GetPersonalFolder();
        string GetMyDocumentsFolder();
        string GetExternalAbsolutePath();
        string GetDeviceIdentification();
        bool IsKeyboardShown();
    }
}
