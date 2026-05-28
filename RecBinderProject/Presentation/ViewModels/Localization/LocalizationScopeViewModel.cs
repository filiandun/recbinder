using RecBinderProject.Infrastructure.LocalizationService;


namespace RecBinderProject.Presentation.ViewModels.Localization
{
    public class LocalizationScopeViewModel
    {
        private readonly LocalizationScope _localizationScope;
        private readonly ILocalizationService _localizationService;

        public LocalizationScopeViewModel(LocalizationScope _localizationScope, ILocalizationService localizationService)
        {
            this._localizationScope = _localizationScope;
            this._localizationService = localizationService;
        }

        public string this[string key] => this._localizationService.GetString(this._localizationScope, key);
    }
}
