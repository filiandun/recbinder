using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RecBinderProject
{
    public partial class RecordPage : Page
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private DispatcherTimer timer;
        public Stopwatch stopWatch;

        private RecordControl recordControl;

        public RecordPage(RecordControl recordControl)
        {
            InitializeComponent();

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.timer = new DispatcherTimer();
            this.stopWatch = new Stopwatch();

            this.timer.Interval = TimeSpan.FromMilliseconds(10);
            this.timer.Tick += Timer_Tick;

            this.recordControl = recordControl;

            this.recordNameTextBox.Text = this.recordControl.RecordingInput.RecordingMetadata.Name;
            this.recordDurationLabel.Content = this.recordControl.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");
            this.recordDateTimeLabel.Content = this.recordControl.RecordingInput.RecordingMetadata.CreationDateTime.ToString();
            this.recordDescriptionTextBox.Text = this.recordControl.RecordingInput.RecordingMetadata.Description;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan elapsed = this.stopWatch.Elapsed;

            this.buttonPlayRecord.Content = elapsed.ToString(@"hh\:mm\:ss\.ff") + " / " + this.recordControl.RecordingInput.RecordingMetadata.Duration.ToString(@"hh\:mm\:ss\.ff");
        }

        private async void ButtonPlayRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.recordNameTextBox.IsEnabled = false;
            this.recordDescriptionTextBox.IsEnabled = false;
            this.recordDurationLabel.IsEnabled = false;
            this.settingsGrid.IsEnabled = false;

            this.buttonPlayRecord.Content = "00:00:00:00";
            this.buttonPlayRecord.IsEnabled = false;
            this.buttonStopRecord.IsEnabled = true;

            this.timer.Start();
            this.stopWatch.Restart();

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            await Task.Run(() => this.recordControl.RecordingInput.InputEvents.PlayEventsAsync(this.cancellationToken));
            ButtonStopRecord_Click(this, e);
        }

        private void ButtonStopRecord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.cancellationTokenSource == null)
            {
                throw new InvalidOperationException();
            }

            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();

            this.timer.Stop();
            this.stopWatch.Stop();

            this.recordNameTextBox.IsEnabled = true;
            this.recordDescriptionTextBox.IsEnabled = true;
            this.recordDurationLabel.IsEnabled = true;
            this.settingsGrid.IsEnabled = true;

            this.buttonPlayRecord.Content = "Воспроизвести запись";
            this.buttonPlayRecord.IsEnabled = true;
            this.buttonStopRecord.IsEnabled = false;
        }
    }
}