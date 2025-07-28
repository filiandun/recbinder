using System.Windows;
using System.Diagnostics;

namespace RecBinderProject.Services
{
    public class PlayRecordService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch _stopwatch;

        private RecordingInput _recordingInput;
        public RecordingInput RecordingInput { get { return this._recordingInput; } }
        public RecordOptions RecordOptions { get; set; }


        public PlayRecordService(string recordName)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();

            this._recordingInput = ((App)Application.Current).RecordingRepository.Read(recordName);

            this.RecordOptions = new RecordOptions();
        }


        public async Task PlayRecord()
        {
            this._stopwatch.Restart();

            this._cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() => this._recordingInput.InputEvents.PlayAsync(this.RecordOptions, this._cancellationTokenSource.Token));
            this.StopRecord();
        }

        public void StopRecord()
        {
            if (this._cancellationTokenSource == null)
            {
                throw new InvalidOperationException();
            }

            this._cancellationTokenSource.Cancel();
            this._cancellationTokenSource.Dispose();

            this._stopwatch.Stop();
        }


        public TimeSpan GetElapsedTime()
        {
            return this._stopwatch.Elapsed;
        }
    }
}
