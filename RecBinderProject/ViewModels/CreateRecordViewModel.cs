using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

using RecBinderProject.Services;


namespace RecBinderProject.ViewModels
{
    internal class CreateRecordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand StartRecordCommand { get; }
        public ICommand StopRecordCommand { get; }


        private bool _isRecording;
        public bool IsRecording
        {
            get => this._isRecording;
            set
            {
                this._isRecording = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsRecording)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsPropertyEnabled)));
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StatusText)));
            }
        }

        private DispatcherTimer _timer;

        private CreateRecordService _createRecordService;
        private RecordOptions _recordOptions;

        private string _recordName;
        private string _recordDescription;
        private double _recordDurationValue;
        private int _recordDurationUnitIndex;


        private bool CanStartRecording() => !this._isRecording;
        private bool CanStopRecording() => this._isRecording;

        public bool IsPropertyEnabled => !this._isRecording;

        public string RecordName
        {
            get => this._recordName;
            set => this.Set(ref this._recordName, value);
        }

        public string RecordDescription
        {
            get => this._recordDescription;
            set => this.Set(ref this._recordDescription, value);
        }

        public double RecordDurationValue
        {
            get => this._recordDurationValue;
            set => this.Set(ref this._recordDurationValue, value);
        }

        public int RecordDurationUnitIndex
        {
            get => this._recordDurationUnitIndex;
            set => this.Set(ref this._recordDurationUnitIndex, value);
        }

        public RecordOptions RecordOptions
        {
            get => this._recordOptions;
        }


        public CreateRecordViewModel()
        {
            this._timer = new DispatcherTimer();
             
            this.IsRecording = false;
            this.StatusText = "Начать запись";

            this._recordOptions = new RecordOptions();
            this._createRecordService = new CreateRecordService(new RecordingMetadata("", "", TimeSpan.MaxValue, DateTime.Now), this._recordOptions);

            this.StartRecordCommand = new RelayCommand(StartRecording, CanStartRecording);
            this.StopRecordCommand = new RelayCommand(HandleRecordingStopped, CanStopRecording);

            this._createRecordService.OnRecordingStopped += CreateRecordService_OnRecordingStopped;
        }
        private void CreateRecordService_OnRecordingStopped()
        {
            this.StopRecording();
        }

        private void StartRecording()
        {
            RecordingMetadata metadata = this.CreateRecordingMetadata();
            this._createRecordService = new CreateRecordService(metadata, this._recordOptions);
            this._createRecordService.StartRecord();

            this.IsRecording = true;
            this.StatusText = "Запись началась";

            this._timer.Interval = TimeSpan.FromMilliseconds(10);
            this._timer.Tick += (s, e) =>
            {
                this.StatusText = this._createRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\:ff") + " / " + metadata.Duration.ToString(@"hh\:mm\:ss\:ff");
            };
            this._timer.Start();

            this.UpdateCommands();
        }

        private void StopRecording()
        {
            this._timer?.Stop();

            this.IsRecording = false;
            this.StatusText = "Запись завершена";

            this.UpdateCommands();
        }

        private void HandleRecordingStopped()
        {
            this._createRecordService.StopRecord();
            
            this.StopRecording();
        }

        public void UpdateCommands()
        {
            (this.StartRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }


        public RecordingMetadata CreateRecordingMetadata()
        {
            TimeSpan recordDuration = this._recordDurationValue > 0 ? this._recordDurationUnitIndex switch
            {
                0 => TimeSpan.FromMilliseconds(this._recordDurationValue),
                1 => TimeSpan.FromSeconds(this._recordDurationValue),
                2 => TimeSpan.FromMinutes(this._recordDurationValue),
                _ => TimeSpan.MaxValue
            } : TimeSpan.MaxValue;

            string recordName = string.IsNullOrWhiteSpace(this._recordName) ? $"RecBinder_{DateTime.Now:g}" : this._recordName;

            return new RecordingMetadata(recordName, this.RecordDescription, recordDuration, DateTime.Now);
        }


        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}

