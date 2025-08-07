using ModernWpf.Controls;
using RecBinderProject.Models;
using RecBinderProject.Services;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;


namespace RecBinderProject.ViewModels
{
    internal class CreateRecordViewModel : RecordViewModel
    {
        public override event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRecording;
        private string _statusText;

        private DispatcherTimer _timer;

        private CreateRecordService _createRecordService;
        private RecordOptions _recordOptions;

        public ICommand StartRecordCommand { get; }
        public ICommand StopRecordCommand { get; }

        private bool CanStartRecording() => !this._isRecording;
        private bool CanStopRecording() => this._isRecording;

        public bool IsRecording // TO DO может чуть изменить? Так как это поле как публичное вообще не используется.
        {
            get => this._isRecording;
            set
            {
                this._isRecording = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsRecording)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsPropertyEnabled)));
            }
        }
       
        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StatusText)));
            }
        }
        public bool IsPropertyEnabled => !this._isRecording;

        public RecordOptions RecordOptions { get => this._recordOptions; }
        public RecordingMetadataViewModel RecordingMetadataViewModel { get; }


        public CreateRecordViewModel(IRecordingRepository recordingRepository)
        {
            this._timer = new DispatcherTimer();
             
            this._isRecording = false;
            this._statusText = "Начать запись";

            this._recordOptions = new RecordOptions();
            this._createRecordService = new CreateRecordService(recordingRepository, this._recordOptions);

            this.RecordingMetadataViewModel = new RecordingMetadataViewModel();

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
            try
            { 
                RecordingMetadata recordingMetadata = this.RecordingMetadataViewModel.ToModel();

                this._createRecordService.RecordingMetadata = recordingMetadata;
                //this._playRecordService.RecordOptions = this.RecordOptions;
                this._createRecordService.StartRecord();

                this.IsRecording = true;
                this.StatusText = "Запись началась";

                this._timer.Interval = TimeSpan.FromMilliseconds(10);
                this._timer.Tick += (s, e) =>
                {
                    this.StatusText = this._createRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\:ff") + " / " + recordingMetadata.Duration.ToString(@"hh\:mm\:ss\:ff");
                };
                this._timer.Start();

                this.UpdateCommands();
            }
            catch (ArgumentException ae)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ae.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ex.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
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

        private void UpdateCommands()
        {
            (this.StartRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}

