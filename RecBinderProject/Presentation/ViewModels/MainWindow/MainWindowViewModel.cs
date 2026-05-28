using System.ComponentModel;
using Microsoft.Extensions.Logging;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.GlobalHotkeyService;
using RecBinderProject.Infrastructure.LocalizationService;

using RecBinderProject.Presentation.Factories;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILogger<MainWindowViewModel> _logger;


        private readonly ILocalizationService _localizationService;

        private readonly IMainWindowViewModelFactory _viewModelFactory;


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

        public MacroListViewModel MacroListViewModel { get; init; }
        public LocalizationViewModel LocalizationViewModel { get; init; }


        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ILocalizationService localizationService, MacroListViewModel macroListViewModel, IMainWindowViewModelFactory viewModelFactory, LocalizationViewModel LocalizationViewModel)
        {
            this._logger = logger;

            this._localizationService = localizationService;

            this.MacroListViewModel = macroListViewModel;

            this._viewModelFactory = viewModelFactory;

            this.LocalizationViewModel = LocalizationViewModel;

            this.MacroListViewModel.RefreshMacros();
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
                        stopRecordingViewModel.StartCommand.Execute(this);
                    }
                    break;

                case HotkeyAction.StartPlaying: 
                    if (this.CurrentViewModel is MacroPlaybackViewModel startPlaybackViewModel)
                    {
                        startPlaybackViewModel.StartCommand.Execute(this);
                    }
                    break;

                case HotkeyAction.StopPlaying:
                    if (this.CurrentViewModel is MacroPlaybackViewModel stopPlaybackViewModel)
                    {
                        stopPlaybackViewModel.StopCommand.Execute(this);
                    }
                    break;

                default: break;
            }
        }

        private void OnMacroRecorded()
        {
            this._logger.LogDebug("On macro recorded");

            this.MacroListViewModel.RefreshMacros();
        }


        public void OpenPlayback(Macro macro)
        {
            this._logger.LogInformation("Opening playback for {MacroName}", macro.Metadata.Name);

            this.CurrentViewModel = this._viewModelFactory.CreatePlayback(macro);
        }

        public void OpenRecording()
        {
            this._logger.LogInformation("Opening recording");

            MacroRecordingViewModel recordingViewModel = this._viewModelFactory.CreateRecording();
            this.CurrentViewModel = recordingViewModel;

            recordingViewModel.MacroRecorded += this.OnMacroRecorded;
        }

        public void OpenHelp()
        {
            this._logger.LogInformation("Opening help");

            //this.CurrentViewModel = this._viewModelFactory.GetHelp();
        }

        public void OpenSettings()
        {
            this._logger.LogInformation("Opening settings");

            this.CurrentViewModel = this._viewModelFactory.GetAppSettings();
        }
    }
}
