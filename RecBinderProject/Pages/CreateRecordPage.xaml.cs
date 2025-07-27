using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using RecBinderProject.Services;

namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        public event Action? OnStopRecording;

        private DispatcherTimer _timer;

        private CreateRecordService _createRecordService;


        public CreateRecordPage(RecordingRepository recordingRepository)
        {
            InitializeComponent();

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(10);
            this._timer.Tick += Timer_Tick;

            this._createRecordService = new CreateRecordService(recordingRepository);
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.buttonStartRecord.Content = this._createRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\.ff");
        }


        private async void ButtonStartRecord_Click(object sender, RoutedEventArgs e)
        {
            this.ControlsEnabled(false, "00:00:00:00");
            this._timer.Start();

            await this._createRecordService.StartRecord(GetRecordingMetadata());

            //this.StopRecord(); // TO DO
        }

        private void ButtonStopRecord_Click(object sender, RoutedEventArgs e)
        {
            this._createRecordService.StopRecord();
            this.StopRecord();
        }


        private void IgnoreKeyboardPressCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._createRecordService.RecordOptions.IgnoreKeyboardPress = this.ignoreKeyboardPressCheckBox.IsChecked ?? false;
        }

        private void IgnoreMousePressCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._createRecordService.RecordOptions.IgnoreMousePress = this.ignoreMousePressCheckBox.IsChecked ?? false;
        }

        private void IgnoreMouseMoveCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._createRecordService.RecordOptions.IgnoreMouseMove = this.ignoreMouseMoveCheckBox.IsChecked ?? false;
        }

        private void IgnoreMouseWheelCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._createRecordService.RecordOptions.IgnoreMouseWheel = this.ignoreMouseWheelCheckBox.IsChecked ?? false;
        }


        private RecordingMetadata GetRecordingMetadata() // TO DO разделит по методам, которые отдельно будут возвращать данные о записи (не забыть и про валидацию данных)
        {
            DateTime recordCreationDateTime = DateTime.Now;

            double durationValue = this.durationNumberBox.Value;
            TimeSpan recordDuration;
            if (durationValue > 0)
            {
                switch (this.durationListBox.SelectedIndex)
                {
                    case 0: recordDuration = TimeSpan.FromMilliseconds(durationValue); break;
                    case 1: recordDuration = TimeSpan.FromSeconds(durationValue); break;
                    case 2: recordDuration = TimeSpan.FromMinutes(durationValue); break;
                    default: recordDuration = TimeSpan.MaxValue; break;
                }
            }
            else
            {
                recordDuration = TimeSpan.MaxValue;
            }
            string recordDesciption = this.recordDescriptionTextBox.Text;
            string recordName = String.IsNullOrEmpty(this.recordNameTextBox.Text) ? $"RecBinder_{recordCreationDateTime.ToString("g")}" : this.recordNameTextBox.Text;

            return new RecordingMetadata(recordName, recordDesciption, recordDuration, recordCreationDateTime);
        }


        private void StopRecord()
        {
            this.ControlsEnabled(true, "Начать запись");

            this.OnStopRecording?.Invoke();
        }


        public void ControlsEnabled(bool value, string buttonStartContent)
        {
            this.recordNameTextBox.IsEnabled = value;
            this.recordDescriptionTextBox.IsEnabled = value;
            this.durationGrid.IsEnabled = value;
            this.settingsGrid.IsEnabled = value;

            this.buttonStartRecord.IsEnabled = value;
            //this.buttonStartRecord.Content = buttonStartContent;
            this.buttonStopRecord.IsEnabled = !value;
        }
    }
}
