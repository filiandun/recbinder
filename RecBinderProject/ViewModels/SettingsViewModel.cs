using ModernWpf;
using ModernWpf.Controls;
using RecBinderProject.Models;
using RecBinderProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RecBinderProject.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isChange;
        private string _statusText;

        private ISettingsService _settingsService;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        private bool CanSave() => this._isChange;
        private bool CanCancel() => this._isChange;
        private bool CanReset() => !this._isChange;

        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StatusText)));
            }
        }
        public bool IsPropertyEnabled => !this._isChange;

        public AppSettingsViewModel AppSettingsViewModel { get; }

        public SettingsViewModel(ISettingsService settingsService)
        {
            this._isChange = false;
            this._statusText = "Настройки сохранены";

            this._settingsService = settingsService;

            this.AppSettingsViewModel = new AppSettingsViewModel(this._settingsService.AppSettings);
            this.AppSettingsViewModel.PropertyChanged += (s, e) =>
            {
                this._isChange = true;
                this._statusText = "Сохранить настройки";

                this.UpdateCommands();

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsPropertyEnabled)));
            };

            this.SaveCommand = new RelayCommand(Save, CanSave);
            this.CancelCommand = new RelayCommand(Cancel, CanCancel);
            this.ResetCommand = new RelayCommand(Reset, CanReset);
        }

        public ApplicationTheme[] GetThemes()
        {
            return (ApplicationTheme[])Enum.GetValues(typeof(ApplicationTheme));
        }

        private async void Save()
        {
            try
            {
                this._settingsService.AppSettings = this.AppSettingsViewModel.ToModel();
                this._settingsService.Save();

                this._isChange = false;
                this._statusText = "Настройки сохранены";

                this.UpdateCommands();
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
            this.AppSettingsViewModel.FromModel(this._settingsService.AppSettings);

            this._isChange = true;
            this._statusText = "Отменять нечего";

            this.UpdateCommands();
        }

        private void Reset()
        {
            this._settingsService.Reset();
            this.AppSettingsViewModel.FromModel(this._settingsService.AppSettings);
            this._settingsService.Save();

            this._isChange = true;
            this._statusText = "Сбрасывать нечего";

            this.UpdateCommands();
        }

        private void UpdateCommands()
        {
            (this.SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.ResetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
