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
    public class PlayRecordViewModel : RecordViewModel
    {
        public override event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRecording;
        private string _statusText;

        private DispatcherTimer _timer;

        private PlayRecordService _playRecordService;
        private RecordOptions _recordOptions;

        public ICommand StartPlayingCommand { get; }
        public ICommand StopPlayingCommand { get; }

        private bool CanStartPlaying() => !this._isRecording;
        private bool CanStopPlaying() => this._isRecording;

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


        public PlayRecordViewModel(IRecordingRepository recordingRepository, string recordName)
        {
            this._timer= new DispatcherTimer();

            this._isRecording = false;
            this._statusText = "Воспроизвести запись";

            this._recordOptions = new RecordOptions();
            this._playRecordService = new PlayRecordService(recordingRepository, this._recordOptions, recordName);

            RecordingMetadata recordingMetadata = this._playRecordService.RecordingInput.RecordingMetadata;
            Debug.WriteLine(this._playRecordService.RecordingInput.RecordingMetadata.Name + "\n" + this._playRecordService.RecordingInput.RecordingMetadata.Description);
            this.RecordingMetadataViewModel = new RecordingMetadataViewModel();
            this.RecordingMetadataViewModel.FromModel(recordingMetadata);

            this.StartPlayingCommand = new RelayCommand(StartPlaying, CanStartPlaying);
            this.StopPlayingCommand = new RelayCommand(HandlePlayingStopped, CanStopPlaying);

            this._playRecordService.OnRecordingStopped += CreateRecordService_OnRecordingStopped;
        }

        private void CreateRecordService_OnRecordingStopped()
        {
            this.StopPlaying();
        }

        private async void StartPlaying()
        {
            try
            {
                RecordingMetadata recordingMetadata = this.RecordingMetadataViewModel.ToModel();

                await this._playRecordService.PlayRecord();

                this.IsRecording = true;
                this.StatusText = "Запись началась";

                this._timer.Interval = TimeSpan.FromMilliseconds(10);
                this._timer.Tick += (s, e) =>
                {
                    string elapsed = this._playRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\.ff");
                    string duration = recordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");

                    this.StatusText = elapsed + " / " + duration;
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
                await contentDialog.ShowAsync();
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

        private void StopPlaying()
        {
            this._timer?.Stop();

            this.IsRecording = false;
            this.StatusText = "Запись завершена";

            this.UpdateCommands();
        }

        private void HandlePlayingStopped()
        {
            this._playRecordService.StopRecord();

            this.StopPlaying();
        }

        private void UpdateCommands()
        {
            (this.StartPlayingCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopPlayingCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
