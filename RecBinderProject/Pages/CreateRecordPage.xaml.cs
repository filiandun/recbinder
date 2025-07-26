using Gma.System.MouseKeyHook;
using RecBinderProject.Inputs;
using RecBinderProject.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        public event EventHandler? OnStopRecording;

        private DispatcherTimer _timer;

        private CreateRecordService _createRecordService;
        private RecordingRepository _recordingRepository;

        public CreateRecordPage(RecordingRepository recordingRepository)
        {
            InitializeComponent();

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(10);
            this._timer.Tick += Timer_Tick;

            this._createRecordService = new CreateRecordService();
            this._recordingRepository = recordingRepository;

            this._createRecordService.OnStopRecording += CreateRecordService_OnStopRecording;
        }

        private void CreateRecordService_OnStopRecording(object? sender, EventArgs e) // TO DO костылииииииииииииии
        {
            //this.ButtonStopRecord_Click(this, new RoutedEventArgs());
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.buttonStartRecord.Content = this._createRecordService.GetElapsedTime().ToString(@"hh\:mm\:ss\.ff");
        }

        private void ButtonStartRecord_Click(object sender, RoutedEventArgs e)
        {
            this.ControlsEnabled(false);
            this.buttonStartRecord.Content = "00:00:00:00";

            this._timer.Start();

            this._createRecordService.StartRecord(GetRecordingMetadata());
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

        private void ButtonStopRecord_Click(object sender, RoutedEventArgs e)
        {
            this.ControlsEnabled(true);
            this.buttonStartRecord.Content = "Начать запись";

            this._createRecordService.StopRecord();
            this._recordingRepository.Save(this._createRecordService.GetRecordingInput());

            this.OnStopRecording?.Invoke(this, e);
        }

        public void ControlsEnabled(bool value)
        {
            this.recordNameTextBox.IsEnabled = value;
            this.recordDescriptionTextBox.IsEnabled = value;
            this.durationGrid.IsEnabled = value;
            this.settingsGrid.IsEnabled = value;

            this.buttonStartRecord.IsEnabled = value;
            this.buttonStopRecord.IsEnabled = !value;
        }
    }
}
