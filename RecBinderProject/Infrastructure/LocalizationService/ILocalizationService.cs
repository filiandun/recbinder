using RecBinderProject.Infrastructure.AppSettingsService;

namespace RecBinderProject.Infrastructure.LocalizationService
{
    public interface ILocalizationService
    {
        public event Action? LanguageChanged;

        public void ApplyLanguage(AppLanguage language);

        public string GetString(string key);
        public string GetString(string key, params object[] args);
    }
}
