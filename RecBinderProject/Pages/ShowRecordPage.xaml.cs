using RecBinderProject.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RecBinderProject
{
    public partial class RecordPage : Page
    {
        private DispatcherTimer _timer;

        private PlayRecordService _playRecordService;
        private RecordingRepository _recordingRepository;

        public RecordPage(RecordingRepository recordingRepository, string recordName)
        {
            InitializeComponent();

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(10);
            this._timer.Tick += Timer_Tick;

            this._recordingRepository = recordingRepository;
            RecordingInput recordingInput = this._recordingRepository.Read(recordName);
            this._playRecordService = new PlayRecordService(recordingInput);

            this.recordNameTextBox.Text = recordingInput.RecordingMetadata.Name;
            this.recordDurationLabel.Content = recordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");
            this.recordDateTimeLabel.Content = recordingInput.RecordingMetadata.CreationDateTime.ToString();
            this.recordDescriptionTextBox.Text = recordingInput.RecordingMetadata.Description;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            string elapsed = this._playRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\.ff");
            //string duration = this._recordControl.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");

            this.buttonPlayRecord.Content = elapsed + " / " + "00:00:00:00"/*duration*/;
        }

        private async void ButtonPlayRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ControlsEnabled(true);
            this.buttonPlayRecord.Content = "00:00:00:00";

            this._timer.Start();
            await this._playRecordService.PlayRecord();
        }

        private void ButtonStopRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._timer.Stop();

            this._playRecordService.StopRecord();

            this.ControlsEnabled(true);
            this.buttonPlayRecord.Content = "Воспроизвести запись";
        }

        public void ControlsEnabled(bool value)
        {
            this.recordNameTextBox.IsEnabled = value;
            this.recordDescriptionTextBox.IsEnabled = value;
            this.recordDurationLabel.IsEnabled = value;
            this.settingsGrid.IsEnabled = value;

            this.buttonPlayRecord.IsEnabled = value;
            this.buttonStopRecord.IsEnabled = !value;
        }
    }
}