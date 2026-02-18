using System.ComponentModel;

using RecBinderProject.Services.LocalizationService;


namespace RecBinderProject.ViewModels
{
    public class LocalizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILocalizationService _localizationService;

        public LocalizationViewModel(ILocalizationService localizationService)
        {
            this._localizationService = localizationService;
            this._localizationService.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public string this[string key] => this._localizationService.GetString(key);
    }

}
