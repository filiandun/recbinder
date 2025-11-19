using RecBinderProject.Models;

namespace RecBinderProject.Services
{
    public class PlayRecordService : BaseRecordService
    {
        private readonly RecordingInput _recordingInput;

        public RecordingInput RecordingInput
        {
            get => this._recordingInput;
        }

        public PlayRecordService(IRecordingRepository recordingRepository, RecordOptions recordOptions, string recordName) : base(recordingRepository, recordOptions)
        {
            this._recordingInput = this._recordingRepository.Read(recordName) ?? throw new ArgumentException($"Record '{recordName}' not found");

            this._isActive = false;
        }


        public async Task PlayRecord()
        {
            this._isActive = true;

            this._stopwatch.Restart();

            this._cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() => this._recordingInput.InputEvents.PlayAsync(this._recordOptions, this._cancellationTokenSource.Token));

            this.StopRecord(); // TO DO колхоз ебучий, надо будет исправить, чтобы PlayAsync callback'ал завершение записи по истечению времени самой записи
        }

        public void StopRecord()
        {
            if (this._isActive)
            {
                this._isActive = false;

                this._cancellationTokenSource.Cancel();
                this._cancellationTokenSource.Dispose();

                this._stopwatch.Stop();

                this.NotifyStopped();
            }
        }
    }
}
