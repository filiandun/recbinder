using System.Resources;
using System.Diagnostics;
using System.Globalization;

using RecBinderProject.Resources;
using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Infrastructure.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        public event Action? LanguageChanged;

        private Dictionary<LocalizationScope, ResourceManager> _resourceManagers;

        private CultureInfo _currentCulture;


        public LocalizationService()
        {
            this._resourceManagers = new Dictionary<LocalizationScope, ResourceManager>()
            {
                { LocalizationScope.MacroList, MacrosList.ResourceManager },

                { LocalizationScope.MacroExecution, MacroExecution.ResourceManager },
                { LocalizationScope.AppSettings, Resources.AppSettings.ResourceManager },
            };

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


        public string GetString(LocalizationScope scope, string key)
        {
            ResourceManager resourceManager = this._resourceManagers.GetValueOrDefault(scope) ?? throw new ArgumentOutOfRangeException($"ResourseManager not found for localization scope {scope}");

            return resourceManager.GetString(key, this._currentCulture) ?? key;
        }
        public string GetString(LocalizationScope scope, string key, params object[] args) => string.Format(this.GetString(scope, key), args);
    }
}