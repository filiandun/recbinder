using System.Windows.Input;
using System.ComponentModel;

using ModernWpf;
using ModernWpf.Controls;

using RecBinderProject.Services.AppSettingsService;


namespace RecBinderProject.ViewModels.AppSettingsViewModels
{
    public class AppSettingsViewModel
    {
        private readonly IAppSettingsService _appSettingsService;

        private List<IAppSettingsSection> _sections;

        public GeneralAppSettingsSectionViewModel GeneralAppSettingsSectionViewModel { get; }
        //public ProfileAppSettingsSectionViewModel ProfileAppSettingsSectionViewModel { get; }
        //public AdvancedAppSettingsSectionViewModel AdvancedAppSettingsSectionViewModel { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        public bool CanSave => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsSaveAllowed);
        public bool CanCancel => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsCancelAllowed);
        public bool CanReset => this._sections.Any(s => s.IsResetAllowed);

        public AppSettingsViewModel(IAppSettingsService settingsService)
        {
            this._appSettingsService = settingsService;

            this._sections = new List<IAppSettingsSection>();

            this.GeneralAppSettingsSectionViewModel = new GeneralAppSettingsSectionViewModel(this._appSettingsService.AppSettings.General); // TODO: VM должны передаваться через DI, а не создаваться тут внутри

            this._sections.Add(this.GeneralAppSettingsSectionViewModel); 

            this.SaveCommand = new RelayCommand(Save, () => this.CanSave);
            this.CancelCommand = new RelayCommand(Cancel, () => this.CanCancel);
            this.ResetCommand = new RelayCommand(Reset, () => this.CanReset);

            foreach (var section in this._sections)
            {
                section.PropertyChanged += OnSectionChanged;

                if (section is INotifyDataErrorInfo errors)
                {
                    errors.ErrorsChanged += OnSectionChanged;
                }
            }
        }

        public void OnSectionChanged(object? sender, EventArgs e)
        {
            (this.SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.ResetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public ApplicationTheme[] GetThemes()
        {
            return (ApplicationTheme[])Enum.GetValues(typeof(ApplicationTheme));
        }


        private async void Save()
        {
            try
            {
                foreach (var section in this._sections)
                {
                    section.Save();
                }

                this.GeneralAppSettingsSectionViewModel.Save();
                this._appSettingsService.AppSettings.General = this.GeneralAppSettingsSectionViewModel.GetValidatedModel();
                this._appSettingsService.Save();
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
            foreach (var section in this._sections)
            {
                section.Cancel();
            }
        }

        private void Reset()
        {
            foreach (var section in this._sections)
            {
                section.Reset();
            }
        }
    }
}
