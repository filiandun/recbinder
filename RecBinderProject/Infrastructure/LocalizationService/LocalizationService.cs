using System.Globalization;

using RecBinderProject.Resources;


namespace RecBinderProject.Infrastructure.LocalizationService
{
    public class LocalizationService : ILocalizationService
    {
        private CultureInfo _currentCulture;

        public event Action? LanguageChanged;

        public LocalizationService()
        {
            this._currentCulture = CultureInfo.InvariantCulture; // TODO связать с настройками (брать оттуда)
        }

        public void SetLanguage(string cultureCode) => this._currentCulture = new CultureInfo(cultureCode);

        public string GetString(string key) => Strings.ResourceManager.GetString(key, this._currentCulture) ?? key;
        public string GetString(string key, params object[] args) => string.Format(GetString(key), args);
    }
}