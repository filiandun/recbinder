using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.Models;
using RecBinderProject.Services.RecordService;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.ViewModels.RecordingMetadataViewModels;


namespace RecBinderProject.ViewModels.RecordViewModels
{
    public abstract class BaseRecordViewModel : INotifyPropertyChanged // TODO подумать над уровнем открытости (public, protected, private) многих полей
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected RecordingMetadataViewModel _recordingMetadataViewModel;

        public LocalizationViewModel LocalizationViewModel { get; init; }

        public RecordingStateViewModel RecordingStateViewModel { get; protected set; }
        public RecordingMetadataViewModel RecordingMetadataViewModel => this._recordingMetadataViewModel;


        protected IRecordProcess _recordProcess;
        protected RecordOptions _recordOptions;

        public ICommand StartCommand { get; protected set; }
        public ICommand PauseCommand { get; protected set; }
        public ICommand StopCommand { get; protected set; }

        protected bool CanStart() => this._recordProcess.State != PlaybackState.Playing; // TODO будто бы логично всё это (состояние) в StateVM переместить (как в AppSettingsVM) (однако, будто бы другой StateVM нужен, а тот переименовать)
        protected bool CanPause() => this._recordProcess.State == PlaybackState.Playing;
        protected bool CanStop() => this._recordProcess.State == PlaybackState.Playing || this._recordProcess.State == PlaybackState.Paused;

        public bool CanEditMetadata => CanStart();

        public RecordOptions RecordOptions { get => this._recordOptions; }


        public BaseRecordViewModel(IRecordProcess recordProcess, ILocalizationService localizationService, string localizationPrefix)
        {
            this._recordingMetadataViewModel = new RecordingMetadataViewModel();

            this.LocalizationViewModel = new LocalizationViewModel(localizationService);

            this._recordProcess = recordProcess;
            this._recordOptions = new RecordOptions();

            this.RecordingStateViewModel = new RecordingStateViewModel(recordProcess, localizationService, localizationPrefix);

            this.StartCommand = new RelayCommand(Start, CanStart);
            this.PauseCommand = new RelayCommand(Pause, CanPause);
            this.StopCommand = new RelayCommand(Stop, CanStop);

            this._recordProcess.StateChanged += this.PlayRecordService_StateChanged;
        }

        private void PlayRecordService_StateChanged(PlaybackState obj)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStart)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanPause)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStop)));

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanEditMetadata)));
        }

        protected abstract void Start();
        protected abstract void Pause();
        protected abstract void Resume();
        protected abstract void Stop();
    }
}
