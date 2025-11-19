using ModernWpf.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

using RecBinderProject.Models;
using RecBinderProject.Services;
using RecBinderProject.ViewModels.RecordingMetadataViewModels;

namespace RecBinderProject.ViewModels.RecordViewModels
{
    public class PlayRecordViewModel : RecordViewModel
    {
        private readonly PlayRecordService _playRecordService;

        public RecordingStateViewModel RecordingStateViewModel { get; }

        public PlayRecordViewModel(IRecordingRepository recordingRepository, string recordName) : base(recordingRepository)
        {
            this._playRecordService = new PlayRecordService(recordingRepository, this._recordOptions, recordName);

            this.RecordingStateViewModel = new RecordingStateViewModel();

            RecordingMetadata recordingMetadata = this._playRecordService.RecordingInput.RecordingMetadata;
            this.RecordingMetadataViewModel.RecordName = recordingMetadata.Name;
            this.RecordingMetadataViewModel.RecordDescription = recordingMetadata.Description;
            this.RecordingMetadataViewModel.RecordDuration = recordingMetadata.Duration;
            this.RecordingMetadataViewModel.RecordCreationDate = recordingMetadata.CreationDateTime;

            this.StartCommand = new RelayCommand(StartPlaying, CanStart);
            this.PauseCommand = new RelayCommand(PausePlaying, CanStart);
            this.StopCommand = new RelayCommand(HandlePlayingStopped, CanStop);

            this._playRecordService.OnStopped += CreateRecordService_OnRecordingStopped;
        }

        private void CreateRecordService_OnRecordingStopped()
        {
            this.StopPlaying();
        }

        private async void StartPlaying()
        {
            try
            {
                this.IsActive = true;

                this._timer.Interval = TimeSpan.FromMilliseconds(10);
                this._timer.Tick += (s, e) =>
                {
                    TimeSpan elapsed = this._playRecordService.GetElapsedTime();
                    TimeSpan duration = this.RecordingMetadataViewModel.RecordDuration;

                    this.RecordingStateViewModel.SetRecordingState(elapsed, duration);
                };
                this._timer.Start();

                await this._playRecordService.PlayRecord();

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

        private void PausePlaying()
        {
            this._timer?.Stop();

            TimeSpan elapsed = this._playRecordService.GetElapsedTime();
            TimeSpan duration = this.RecordingMetadataViewModel.RecordDuration;
            this.RecordingStateViewModel.SetPausedState(elapsed, duration);

            this.UpdateCommands();
        }

        private void StopPlaying()
        {
            this._timer?.Stop();

            this.IsActive = false;

            TimeSpan elapsed = this._playRecordService.GetElapsedTime();
            TimeSpan duration = this.RecordingMetadataViewModel.RecordDuration;
            this.RecordingStateViewModel.SetStoppedState(elapsed, duration);

            this.UpdateCommands();
        }


        private void HandlePlayingStopped()
        {
            this._playRecordService.StopRecord();

            this.StopPlaying();
        }

        private void UpdateCommands()
        {
            (this.StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
