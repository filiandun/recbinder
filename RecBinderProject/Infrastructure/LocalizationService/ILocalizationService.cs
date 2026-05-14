namespace RecBinderProject.Infrastructure.LocalizationService
{
    public interface ILocalizationService
    {
        event Action? LanguageChanged;

        void SetLanguage(string cultureCode);

        string GetString(string key);
        string GetString(string key, params object[] args);
    }
}
