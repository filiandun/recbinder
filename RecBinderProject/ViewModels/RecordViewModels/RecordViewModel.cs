using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Threading;

using RecBinderProject.Models;
using RecBinderProject.Services;
using RecBinderProject.ViewModels.RecordingMetadataViewModels;

namespace RecBinderProject.ViewModels.RecordViewModels
{
    public abstract class RecordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected readonly RecordingMetadataViewModel _recordingMetadataViewModel;

        protected DispatcherTimer _timer;

        protected RecordOptions _recordOptions;
        protected IRecordingRepository _recordingRepository;

        protected bool _isActive;
        protected string _statusText;

        public ICommand StartCommand { get; protected set; }
        public ICommand PauseCommand { get; protected set; }
        public ICommand StopCommand { get; protected set; }

        protected bool CanStart() => !this._isActive;
        protected bool CanStop() => this._isActive;
      
        public bool IsActive // TODO может чуть изменить? Так как это поле как публичное вообще не используется.
        {
            get => this._isActive;
            protected set
            {
                this._isActive = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanStart)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanStop)));
            }
        }

        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
            }
        }
        //public bool IsPropertyEnabled => this._isActive;

        public RecordOptions RecordOptions { get => this._recordOptions; }
        public RecordingMetadataViewModel RecordingMetadataViewModel => this._recordingMetadataViewModel;

        public RecordViewModel(IRecordingRepository recordingRepository)
        {
            this._recordingMetadataViewModel = new RecordingMetadataViewModel();

            this._timer = new DispatcherTimer();

            this._recordOptions = new RecordOptions();
            this._recordingRepository = recordingRepository;

            this._isActive = false;
            this._statusText = string.Empty;

            //this.StartCommand = new RelayCommand(StartRecording, CanStart); // TODO
            //this.StopCommand = new RelayCommand(HandleRecordingStopped, CanStop);
        }
    }
}
