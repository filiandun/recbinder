using RecBinderProject.Infrastructure.AppSettingsService;

namespace RecBinderProject.Infrastructure.LocalizationService
{
    public interface ILocalizationService
    {
        public event Action? LanguageChanged;

        public void ApplyLanguage(AppLanguage language);

        public string GetString(LocalizationScope scope, string key);
        public string GetString(LocalizationScope scope, string key, params object[] args);
    }
}
