using System.Resources;
using System.Globalization;
using Microsoft.Extensions.Logging;

using RecBinderProject.Infrastructure.Services.AppSettings;
using RecBinderProject.ApplicationLayer.Localization;


namespace RecBinderProject.Infrastructure.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        public event Action? LanguageChanged;

        private readonly ILogger<LocalizationService> _logger;

        private Dictionary<LocalizationScope, ResourceManager> _resourceManagers;
        private CultureInfo _currentCulture;


        public LocalizationService(ILogger<LocalizationService> logger)
        {
            this._logger = logger;

            this._resourceManagers = new Dictionary<LocalizationScope, ResourceManager>()
            {
                { LocalizationScope.Common, Resources.Localization.Common.ResourceManager },

                { LocalizationScope.MainWindow, Resources.Localization.MainWindow.ResourceManager },

                { LocalizationScope.MacroList, Resources.Localization.MacroList.ResourceManager },
                { LocalizationScope.MacroExecution, Resources.Localization.MacroExecution.ResourceManager },

                { LocalizationScope.AppSettings, Resources.Localization.AppSettings.ResourceManager },
            };

            this._currentCulture = this.GetCultureInfo(AppLanguage.English);
        }


        public void ApplyLanguage(AppLanguage language) 
        {
            this._currentCulture = this.GetCultureInfo(language);

            this._logger.LogInformation("Application language switched to {Language} ({Culture})", language, this._currentCulture.Name);

            this.LanguageChanged?.Invoke();
        }


        public string GetString(LocalizationScope scope, string key)
        {
            if (!this._resourceManagers.TryGetValue(scope, out ResourceManager? resourceManager))
            {
                this._logger.LogError("ResourceManager not found for localization scope '{Scope}'.", scope);

                throw new ArgumentOutOfRangeException($"ResourceManager not found for localization scope {scope}");
            }

            string? value = resourceManager.GetString(key, this._currentCulture);

            if (value is null)
            {
                this._logger.LogWarning("Localization key '{Key}' not found in scope '{Scope}' for culture '{Culture}'. Falling back to key name.", key, scope, this._currentCulture.Name);

                return key;
            }

            return value;
        }

        public string GetString(LocalizationScope scope, string key, params object[] args) => string.Format(this.GetString(scope, key), args);


        private CultureInfo GetCultureInfo(AppLanguage language)
        {
            string cultureCode = language switch
            {
                AppLanguage.English => "en-US",
                AppLanguage.Russian => "ru-RU",

                _ => "en-US"
            };

            return CultureInfo.GetCultureInfo(cultureCode);
        }
    }
}