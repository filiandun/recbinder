using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject.Services
{
    public class PlayRecordService
    {
        private CancellationTokenSource _cancellationTokenSource;

        private Stopwatch _stopwatch;

        private RecordingInput _recordingInput;

        public PlayRecordService(RecordingInput recordingInput)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();

            this._recordingInput = recordingInput;
        }

        public async Task PlayRecord()
        {
            this._stopwatch.Restart();

            this._cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() => this._recordingInput.InputEvents.PlayAsync(this._cancellationTokenSource.Token));
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
