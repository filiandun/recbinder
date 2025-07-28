using System.Windows.Controls;
using System.Windows.Threading;

using RecBinderProject.Services;

namespace RecBinderProject
{
    public partial class ShowRecordPage : Page
    {
        private DispatcherTimer _timer;

        private PlayRecordService _playRecordService;


        public ShowRecordPage(string recordName)
        {
            InitializeComponent();

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(10);
            this._timer.Tick += Timer_Tick;

            this._playRecordService = new PlayRecordService(recordName);

            this.recordNameTextBox.Text = this._playRecordService.RecordingInput.RecordingMetadata.Name;
            this.recordDurationLabel.Content = this._playRecordService.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");
            this.recordDateTimeLabel.Content = this._playRecordService.RecordingInput.RecordingMetadata.CreationDateTime.ToString();
            this.recordDescriptionTextBox.Text = this._playRecordService.RecordingInput.RecordingMetadata.Description;
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            string elapsed = this._playRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\.ff");
            string duration = this._playRecordService.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");

            this.buttonPlayRecord.Content = elapsed + " / " + duration;
        }


        private async void ButtonPlayRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ControlsEnabled(true, "00:00:00:00");
            this._timer.Start();

            await this._playRecordService.PlayRecord();

            this.OnStopRecord();
        }

        private void ButtonStopRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._playRecordService.StopRecord();
            this.OnStopRecord();
        }


        private void IgnoreKeyboardPressCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._playRecordService.RecordOptions.IgnoreKeyboardPress = this.ignoreKeyboardPressCheckBox.IsChecked ?? false;
        }

        private void IgnoreMousePressCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._playRecordService.RecordOptions.IgnoreMousePress = this.ignoreMousePressCheckBox.IsChecked ?? false;
        }

        private void IgnoreMouseMoveCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._playRecordService.RecordOptions.IgnoreMouseMove = this.ignoreMouseMoveCheckBox.IsChecked ?? false;
        }

        private void IgnoreMouseWheelCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._playRecordService.RecordOptions.IgnoreMouseWheel = this.ignoreMouseWheelCheckBox.IsChecked ?? false;
        }


        private void OnStopRecord()
        {
            this._timer.Stop();
            this.ControlsEnabled(true, "Воспроизвести запись");
        }

        public void ControlsEnabled(bool value, string buttonPlayContent)
        {
            this.recordNameTextBox.IsEnabled = value;
            this.recordDescriptionTextBox.IsEnabled = value;
            this.recordDurationLabel.IsEnabled = value;
            this.settingsGrid.IsEnabled = value;

            this.buttonPlayRecord.Content = buttonPlayContent;
            this.buttonPlayRecord.IsEnabled = value;
            this.buttonStopRecord.IsEnabled = !value;
        }
    }
}