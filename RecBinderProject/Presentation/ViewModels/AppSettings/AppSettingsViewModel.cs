using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Presentation.ViewModels.AppSettings
{
    public class AppSettingsViewModel
    {
        private readonly IAppSettingsService _appSettingsService;

        private readonly List<IAppSettingsSectionViewModel> _sections;

        public LocalizationViewModel LocalizatonViewModel { get; init; }

        public GeneralAppSettingsSectionViewModel GeneralAppSettingsSectionViewModel { get; }
        //public ProfileAppSettingsSectionViewModel ProfileAppSettingsSectionViewModel { get; }
        //public AdvancedAppSettingsSectionViewModel AdvancedAppSettingsSectionViewModel { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        public bool CanSave => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsSaveAllowed);
        public bool CanCancel => this._sections.Any(s => s.HasEdits) && this._sections.All(s => s.IsCancelAllowed);
        public bool CanReset => this._sections.Any(s => s.IsResetAllowed);

        public AppSettingsViewModel(IAppSettingsService settingsService, LocalizationViewModel LocalizatonViewModel, GeneralAppSettingsSectionViewModel generalAppSettingsSectionViewModel)
        {
            this._appSettingsService = settingsService;

            this._sections = new List<IAppSettingsSectionViewModel>();

            this.LocalizatonViewModel = LocalizatonViewModel;

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


        private async void OnSaveCommand()
        {
            try
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
            catch (Exception ex)
            {
                //ContentDialog contentDialog = new ContentDialog() // TODO сделать менеджер-сервис contentDialog'ов, чтобы там и локализация подтяигвались, да и чтобы так много текста не было для оторбражения
                //{
                //    Content = ex.Message,
                //    CloseButtonText = "Закрыть",
                //};
                //await contentDialog.ShowAsync();
            }
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
