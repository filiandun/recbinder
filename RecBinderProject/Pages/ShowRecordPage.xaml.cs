using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RecBinderProject
{
    public partial class RecordPage : Page
    {
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private Stopwatch _stopwatch;
        private DispatcherTimer _dispatcherTimer;

        private RecordControl _recordControl;

        public RecordPage(RecordControl recordControl)
        {
            InitializeComponent();

            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = this._cancellationTokenSource.Token;

            this._stopwatch = new Stopwatch();

            this._dispatcherTimer = new DispatcherTimer();
            this._dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            this._dispatcherTimer.Tick += Timer_Tick;

            this._recordControl = recordControl;

            this.recordNameTextBox.Text = this._recordControl.RecordingInput.RecordingMetadata.Name;
            this.recordDurationLabel.Content = this._recordControl.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");
            this.recordDateTimeLabel.Content = this._recordControl.RecordingInput.RecordingMetadata.CreationDateTime.ToString();
            this.recordDescriptionTextBox.Text = this._recordControl.RecordingInput.RecordingMetadata.Description;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            string elapsed = this._stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
            string duration = this._recordControl.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");

            this.buttonPlayRecord.Content = elapsed + " / " + duration;
        }


        private async void ButtonPlayRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ControlsEnabled(true);
            this.buttonPlayRecord.Content = "00:00:00:00";

            this._dispatcherTimer.Start();
            this._stopwatch.Restart();

            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = this._cancellationTokenSource.Token;

            await Task.Run(() => this._recordControl.RecordingInput.InputEvents.PlayAsync(this._cancellationToken));
            this.ButtonStopRecord_Click(this, e);
        }

        private void ButtonStopRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._cancellationTokenSource == null)
            {
                throw new InvalidOperationException();
            }

            this._cancellationTokenSource.Cancel();
            this._cancellationTokenSource.Dispose();

            this._dispatcherTimer.Stop();
            this._stopwatch.Stop();

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