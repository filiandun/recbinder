using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Extensions.Logging;

using RecBinderProject.Infrastructure.GlobalHotkeyService;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.Factories;
using RecBinderProject.Presentation.Services;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILogger<MainWindowViewModel> _logger;

        public LocalizationViewModel LocalizationViewModel { get; private set; }

        public IDialogService DialogService { get; private set; }

        private readonly IMainWindowViewModelFactory _viewModelFactory;

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

        public ICommand OpenSettingsCommand { get; private set; }
        public ICommand OpenHelpCommand { get; private set; }


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


        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IMainWindowViewModelFactory viewModelFactory, LocalizationViewModel LocalizationViewModel, IDialogService dialogService)
        {
            this._logger = logger;

            this._viewModelFactory = viewModelFactory;

            this._currentViewModel = this._viewModelFactory.CreateLibrary();

            this.LocalizationViewModel = LocalizationViewModel;

            this.DialogService = dialogService;

            this._selectedMenuIndex = 0;
            this.OpenSettingsCommand = new RelayCommand(this.ExecuteOpenSettings);
            this.OpenHelpCommand = new RelayCommand(this.ExecuteOpenHelp);
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

        //private void OnMacroRecorded()
        //{
        //    this._logger.LogDebug("On macro recorded");

        //    //this.MacroListViewModel.RefreshMacros();
        //}


        public void ExecuteOpenLibrary()
        {
            this._logger.LogInformation("Opening library");

            MacroLibraryViewModel libraryViewModel = this._viewModelFactory.CreateLibrary();
            this.CurrentViewModel = libraryViewModel;
        }

        public void ExecuteOpenRecording()
        {
            this._logger.LogInformation("Opening recording");

            MacroRecordingViewModel recordingViewModel = this._viewModelFactory.CreateRecording();
            this.CurrentViewModel = recordingViewModel;

            //recordingViewModel.MacroRecorded += this.OnMacroRecorded;
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
            this.CurrentViewModel = this._viewModelFactory.CreateAppSettings();
        }
    }
}
