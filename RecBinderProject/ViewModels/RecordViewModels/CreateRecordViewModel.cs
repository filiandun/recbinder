using ModernWpf.Controls;
using RecBinderProject.Models;
using RecBinderProject.Services;
using RecBinderProject.ViewModels.RecordingMetadataViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace RecBinderProject.ViewModels.RecordViewModels
{
    internal class CreateRecordViewModel : RecordViewModel
    {
        private CreateRecordService? _createRecordService;

        public CreateRecordViewModel(IRecordingRepository recordingRepository) : base(recordingRepository)
        {
            this._statusText = "Начать запись";

            this._createRecordService = null;

            this.StartCommand = new RelayCommand(StartRecording, CanStart);
            this.StopCommand = new RelayCommand(HandleRecordingStopped, CanStop);
        }

        private void CreateRecordService_OnRecordingStopped()
        {
            StopRecording();
        }

        private void StartRecording()
        {
            try
            {
                this.RecordingMetadataViewModel.ApplyChanges();
                RecordingMetadata recordingMetadata = this.RecordingMetadataViewModel.RecordingMetadataModel;
                MessageBox.Show($"metadata:\n name: {recordingMetadata.Name}\ndecription: {recordingMetadata.Description}\nduration {recordingMetadata.Duration}\n date {recordingMetadata.CreationDateTime}");

                this._createRecordService = new CreateRecordService(this._recordingRepository, recordingMetadata, this.RecordOptions);
                this._createRecordService.OnStopped += CreateRecordService_OnRecordingStopped;
                this._createRecordService.StartRecord();

                this.IsActive = true;
                this.StatusText = "Запись началась";

                this._timer.Interval = TimeSpan.FromMilliseconds(10);
                this._timer.Tick += (s, e) =>
                {
                    this.StatusText = this._createRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\:ff") + " / " + recordingMetadata.Duration.ToString(@"hh\:mm\:ss\:ff");
                };
                this._timer.Start();

                UpdateCommands();
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

            this.IsActive = false;
            this.StatusText = "Запись завершена";

            UpdateCommands();
        }

        private void HandleRecordingStopped()
        {
            this._createRecordService.StopRecord();
            
            StopRecording();
        }

        private void UpdateCommands()
        {
            (this.StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}

