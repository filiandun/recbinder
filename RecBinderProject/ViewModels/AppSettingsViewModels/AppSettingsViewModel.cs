using ModernWpf;
using ModernWpf.Controls;
using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.Services.AppSettingsService;


namespace RecBinderProject.ViewModels.AppSettingsViewModels
{
    public class AppSettingsViewModel
    {
        private readonly IAppSettingsService _settingsService;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        public AppSettingsEditorViewModel AppSettingsEditorViewModel { get; init; }
        public AppSettingsStateViewModel AppSettingsStateViewModel { get; init; }

        public AppSettingsViewModel(IAppSettingsService settingsService)
        {
            this._settingsService = settingsService;

            this.AppSettingsEditorViewModel = new AppSettingsEditorViewModel(this._settingsService.AppSettings);
            this.AppSettingsStateViewModel = new AppSettingsStateViewModel(this.AppSettingsEditorViewModel);

            this.SaveCommand = new RelayCommand(Save, this.AppSettingsStateViewModel.CanSave);
            this.CancelCommand = new RelayCommand(Cancel, this.AppSettingsStateViewModel.CanCancel);
            this.ResetCommand = new RelayCommand(Reset, this.AppSettingsStateViewModel.CanReset);
        }

        public ApplicationTheme[] GetThemes()
        {
            return (ApplicationTheme[])Enum.GetValues(typeof(ApplicationTheme));
        }

        private async void Save()
        {
            try
            {
                this.AppSettingsEditorViewModel.Save();
                this._settingsService.AppSettings = this.AppSettingsEditorViewModel.GetValidatedModel();
                this._settingsService.Save();
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ex.Message,
                    CloseButtonText = "Закрыть",
                };
                await contentDialog.ShowAsync();
            }
        }

        private void Cancel()
        {
            this.AppSettingsEditorViewModel.Cancel();
        }

        private void Reset()
        {
            this.AppSettingsEditorViewModel.Reset();
        }
    }
}
