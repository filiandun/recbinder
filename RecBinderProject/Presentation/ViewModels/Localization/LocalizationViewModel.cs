using System.ComponentModel;

using RecBinderProject.ApplicationLayer.Localization;


namespace RecBinderProject.Presentation.ViewModels.Localization
{
    public class LocalizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public LocalizationScopeViewModel Common { get; }
        public LocalizationScopeViewModel AppSettings { get; }
        public LocalizationScopeViewModel MacroList { get; }
        public LocalizationScopeViewModel MacroExecution { get; }

        public LocalizationViewModel(ILocalizationService localizationService)
        {
            Common = new LocalizationScopeViewModel(LocalizationScope.Common, localizationService);
            AppSettings = new LocalizationScopeViewModel(LocalizationScope.AppSettings, localizationService);
            MacroList = new LocalizationScopeViewModel(LocalizationScope.MainWindow, localizationService);
            MacroExecution = new LocalizationScopeViewModel(LocalizationScope.MacroExecution, localizationService);

            localizationService.LanguageChanged += this.OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
