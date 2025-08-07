using RecBinderProject.Models;
using System.Diagnostics;

namespace RecBinderProject.Services
{
    public class PlayRecordService
    {
        public event Action? OnRecordingStopped;

        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch _stopwatch;

        private IRecordingRepository _recordingRepository;

        private RecordingInput _recordingInput;
        public RecordingInput RecordingInput { get { return this._recordingInput; } }
        public RecordOptions RecordOptions { get; set; }

        private bool _isPlaying;

        public PlayRecordService(IRecordingRepository recordingRepository, RecordOptions recordOptions, string recordName)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();

            this._recordingRepository = recordingRepository;

            this._recordingInput = this._recordingRepository.Read(recordName);

            this.RecordOptions = recordOptions;

            this._isPlaying = false;
        }


        public async Task PlayRecord()
        {
            this._isPlaying = true;

            this._stopwatch.Restart();

            this._cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() => this._recordingInput.InputEvents.PlayAsync(this.RecordOptions, this._cancellationTokenSource.Token));

            this.StopRecord();
        }

        public void StopRecord()
        {
            if (this._isPlaying)
            {
                this._isPlaying = false;

                this._cancellationTokenSource.Cancel();
                this._cancellationTokenSource.Dispose();

                this._stopwatch.Stop();

                this.OnRecordingStopped?.Invoke(); // TO DO nullreferenceexception
            }
        }


        public TimeSpan GetElapsedTime()
        {
            return this._stopwatch.Elapsed;
        }
    }
}
