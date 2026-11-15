using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.Services.Dialog;
using System.Runtime.CompilerServices;
using RecBinderProject.ApplicationLayer.Localization;
using RecBinderProject.ApplicationLayer.AppSettings;


namespace RecBinderProject.Presentation.ViewModels.AppSettings
{
    public class AppSettingsViewModel
    {
        private readonly ILocalizationService _localizationService;
        public LocalizationViewModel LocalizatonViewModel { get; private set; }

        private readonly IDialogService _dialogService;

        private readonly IAppSettingsService _appSettingsService;

        private readonly List<IAppSettingsSectionViewModel> _sections;


        public GeneralAppSettingsSectionViewModel GeneralAppSettingsSectionViewModel { get; }
        //public ProfileAppSettingsSectionViewModel ProfileAppSettingsSectionViewModel { get; }
        //public AdvancedAppSettingsSectionViewModel AdvancedAppSettingsSectionViewModel { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        public bool CanSave => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsSaveAllowed);
        public bool CanCancel => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsCancelAllowed);
        public bool CanReset => this._sections.Any(s => s.IsResetAllowed);

        public AppSettingsViewModel(ILocalizationService localizationService, LocalizationViewModel LocalizatonViewModel, IDialogService dialogService, IAppSettingsService settingsService, GeneralAppSettingsSectionViewModel generalAppSettingsSectionViewModel)
        {
            this.LocalizatonViewModel = LocalizatonViewModel;
            this._localizationService = localizationService;

            this._dialogService = dialogService;

            this._appSettingsService = settingsService;

            this._sections = new List<IAppSettingsSectionViewModel>();

            this.GeneralAppSettingsSectionViewModel = generalAppSettingsSectionViewModel;
            //this.ProfileAppSettingsSectionViewModel = profileAppSettingsSectionViewModel;
            //this.AdvancedAppSettingsSectionViewModel = advancedAppSettingsSectionViewModel;

            this._sections.Add(this.GeneralAppSettingsSectionViewModel);
            //this._sections.Add(this.ProfileAppSettingsSectionViewModel);
            //this._sections.Add(this.AdvancedAppSettingsSectionViewModel);

            this.SaveCommand = new RelayCommand(this.OnSaveCommand, () => this.CanSave);
            this.CancelCommand = new RelayCommand(this.OnCancelCommand, () => this.CanCancel);
            this.ResetCommand = new RelayCommand(this.OnResetCommand, () => this.CanReset);

            foreach (var section in this._sections)
            {
                section.PropertyChanged += this.OnSectionChanged;

                if (section is INotifyDataErrorInfo errors)
                {
                    errors.ErrorsChanged += this.OnSectionChanged;
                }
            }
        }

        public async Task InitializeAsync()
        {
            if (this._appSettingsService.LoadState == AppSettingsLoadState.LoadedFromFile) return;

            string messageKey = this._appSettingsService.LoadState switch
            {
                AppSettingsLoadState.MissingFile => "Dialog_Load_MissingFile",
                AppSettingsLoadState.CorruptedFile => "Dialog_Load_CorruptedFile",
                AppSettingsLoadState.AccessDenied => "Dialog_Load_AccessDenied",

                _ => "Dialog_Load_UnknownError"
            };

            string title = this._localizationService.GetString(LocalizationScope.AppSettings, "Dialog_Load_TitleAttention");
            string message = this._localizationService.GetString(LocalizationScope.AppSettings, messageKey);

            await this._dialogService.ShowAsync(title, message);
        }


        public void OnSectionChanged(object? sender, EventArgs e)
        {
            (this.SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.ResetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }


        private async void OnSaveCommand()
        {
            foreach (var section in this._sections) // PS: два foreach типо для атомарности
            {
                section.SaveLocal();
            }

            foreach (var section in this._sections)
            {
                section.ApplyToService();
            }

            this._appSettingsService.Save();
        }

        private void OnCancelCommand()
        {
            foreach (var section in this._sections)
            {
                section.CancelLocal();
            }
        }

        private void OnResetCommand()
        {
            foreach (var section in this._sections)
            {
                section.ResetLocal();
            }
        }
    }
}
