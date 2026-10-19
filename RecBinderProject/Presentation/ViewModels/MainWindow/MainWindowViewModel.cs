using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Extensions.Logging;

using RecBinderProject.Infrastructure.Services.GlobalHotkey;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.Services.Dialog;
using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroLibrary;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IDialogService _dialogService;

        private readonly MacroLibraryViewModel _macroLibraryViewModel;
        private readonly MacroRecordingViewModel _macroRecordingViewModel;
        private readonly AppSettingsViewModel _appSettingsViewModel;

        public LocalizationViewModel LocalizationViewModel { get; private set; }

        public ICommand OpenSettingsCommand { get; private set; }
        public ICommand OpenHelpCommand { get; private set; }


        private int _selectedMenuIndex;
        public int SelectedMenuIndex 
        {
            get => this._selectedMenuIndex;
            set
            {
                if (this._selectedMenuIndex == value) return;

                this._selectedMenuIndex = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedMenuIndex)));

                switch (value)
                {
                    case 0: this.ExecuteOpenLibrary(); break;
                    case 1: this.ExecuteOpenRecording(); break;
                }
            }
        }

        private object? _currentViewModel;
        public object? CurrentViewModel
        {
            get => this._currentViewModel;
            set
            {
                if (Equals(this._currentViewModel, value)) return;

                this._currentViewModel = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentViewModel)));
            }
        }


        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, LocalizationViewModel LocalizationViewModel, IDialogService dialogService, MacroLibraryViewModel macroLibraryViewModel, MacroRecordingViewModel macroRecordingViewModel, AppSettingsViewModel appSettingsViewModel)
        {
            this._logger = logger;
            this._dialogService = dialogService;

            this.LocalizationViewModel = LocalizationViewModel;

            this._macroLibraryViewModel = macroLibraryViewModel;
            this._macroRecordingViewModel = macroRecordingViewModel;
            this._appSettingsViewModel = appSettingsViewModel;


            this.OpenSettingsCommand = new RelayCommand(this.ExecuteOpenSettings);
            this.OpenHelpCommand = new RelayCommand(this.ExecuteOpenHelp);

            this._selectedMenuIndex = 0;
            this._currentViewModel = macroLibraryViewModel;
        }


        public async Task InitializeAsync()
        {
            await this._appSettingsViewModel.InitializeAsync();

            await this._macroLibraryViewModel.InitializeAsync();
        }


        public void OnHotkeyPressed(HotkeyAction action)
        {
            this._logger.LogDebug("Hotkey action: {HotkeyAction}", action.ToString());

            switch (action)
            {
                case HotkeyAction.StartRecording:
                    if (this.CurrentViewModel is MacroRecordingViewModel startRecordingViewModel)
                    {
                        startRecordingViewModel.StartCommand.Execute(this);
                    }
                    break;

                case HotkeyAction.StopRecording:
                    if (this.CurrentViewModel is MacroRecordingViewModel stopRecordingViewModel)
                    {
                        stopRecordingViewModel.StopCommand.Execute(this);
                    }
                    break;

                case HotkeyAction.StartPlaying: 
                    if (this.CurrentViewModel is MacroLibraryViewModel startLibraryViewModel)
                    {
                        startLibraryViewModel.MacroPlaybackViewModel?.StartCommand.Execute(this);
                    }
                    break;

                case HotkeyAction.StopPlaying:
                    if (this.CurrentViewModel is MacroLibraryViewModel stopLibraryViewModel)
                    {
                        stopLibraryViewModel.MacroPlaybackViewModel?.StopCommand.Execute(this);
                    }
                    break;

                default: break;
            }
        }


        public void ExecuteOpenLibrary()
        {
            this._logger.LogInformation("Opening library");

            this.CurrentViewModel = this._macroLibraryViewModel;
        }

        public void ExecuteOpenRecording()
        {
            this._logger.LogInformation("Opening recording");

            this.CurrentViewModel = this._macroRecordingViewModel;
        }

        public void ExecuteOpenHelp()
        {
            this._logger.LogInformation("Opening help");

            //this.SelectedMenuIndex = -1;
            //this.CurrentViewModel = this._viewModelFactory.GetHelp();
        }

        public void ExecuteOpenSettings()
        {
            this._logger.LogInformation("Opening settings");

            this.SelectedMenuIndex = -1;
            this.CurrentViewModel = this._appSettingsViewModel;
        }
    }
}
