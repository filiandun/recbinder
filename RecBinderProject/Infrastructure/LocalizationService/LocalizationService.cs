using System.Globalization;

using RecBinderProject.Resources;
using RecBinderProject.Infrastructure.AppSettingsService;
using System.Diagnostics;


namespace RecBinderProject.Infrastructure.LocalizationService
{
    public class LocalizationService : ILocalizationService
    {
        public event Action? LanguageChanged;

        private CultureInfo _currentCulture;


        public LocalizationService()
        {
            this._currentCulture = CultureInfo.InvariantCulture; // TODO связать с настройками (брать оттуда)
        }


        public void ApplyLanguage(AppLanguage language) 
        {
            string cultureCode = language switch
            {
                AppLanguage.English => "en-US",
                AppLanguage.Russian => "ru-RU",

                _ => CultureInfo.CurrentUICulture.Name // TODO тут бы сообщать пользователю, что фигня какая-то
            };

            Debug.WriteLine($"APPLY LANGUAGE {cultureCode}");

            this._currentCulture = new CultureInfo(cultureCode);
            this.LanguageChanged?.Invoke();
        }


        public string GetString(string key) => Strings.ResourceManager.GetString(key, this._currentCulture) ?? key;
        public string GetString(string key, params object[] args) => string.Format(GetString(key), args);
    }
}