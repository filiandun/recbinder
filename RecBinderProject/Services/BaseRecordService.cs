using System.Diagnostics;

using RecBinderProject.Models;

namespace RecBinderProject.Services
{
    public class BaseRecordService
    {
        public event Action? OnStopped;

        protected CancellationTokenSource _cancellationTokenSource;
        protected readonly Stopwatch _stopwatch;

        protected readonly IRecordingRepository _recordingRepository;

        protected RecordOptions _recordOptions;

        protected bool _isActive;


        public BaseRecordService(IRecordingRepository recordingRepository, RecordOptions recordOptions)
        {
            this.OnStopped = null;

            this._cancellationTokenSource = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();

            this._recordingRepository = recordingRepository;

            this._recordOptions = recordOptions;

            this._isActive = false;
        }


        protected void NotifyStopped() => this.OnStopped?.Invoke();  // TO DO nullreferenceexception

        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;
    }
}
