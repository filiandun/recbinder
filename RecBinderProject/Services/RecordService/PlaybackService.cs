using System.Diagnostics;

using RecBinderProject.Models;


namespace RecBinderProject.Services.RecordService
{
    public class PlaybackService : BaseRecordService
    {
        private readonly SynchronizationContext _syncContext;

        private Thread _playbackThread;

        private readonly RecordingInput _recordingInput; // TODO имеет смысл хранить не саму запись, а только события из неё (так как метаданные тут не используются)
        private readonly InputPlayer _inputPlayer;

        public override TimeSpan TotalTime => this._recordingInput.RecordingMetadata.Duration;
        public override TimeSpan ElapsedTime => this._inputPlayer.GetElapsedTime();

        public override RecordingMetadata RecordingMetadata => this._recordingInput.RecordingMetadata;

        public PlaybackService(RecordOptions recordOptions, RecordingInput recordingInput) : base(recordOptions)
        {
            this._syncContext = SynchronizationContext.Current ?? throw new NullReferenceException("SynchronizationContext is null"); // TODO null

            this._playbackThread = new Thread(PlaybackWorker); // TODO заглушка (смысла в этой инициализации 0, только чтобы не ругалось на отсутствие инициализации в конструкторе)

            this._recordingInput = recordingInput;
            this._inputPlayer = new InputPlayer(this._recordingInput, recordOptions);
        }

        public override void Start()    
        {
            if (this.State == PlaybackState.Stopped || this.State == PlaybackState.Completed) 
            {
                this.State = PlaybackState.Playing;

                this._playbackThread = new Thread(PlaybackWorker)
                {
                    Name = "PlaybackThread",
                    IsBackground = true
                };
                this._playbackThread.Start();
            }
        }

        public override void Pause()
        {
            if (this.State == PlaybackState.Playing)
            {
                this.State = PlaybackState.Paused;
                this._inputPlayer.Pause();
            }
        }

        public override void Resume()
        {
            if (this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Playing;
                this._inputPlayer.Resume();
            }
        }

        public override void Stop()
        {
            if (this.State == PlaybackState.Playing || this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Stopped;
                this._inputPlayer.Stop();
            }
        }

        private void PlaybackWorker()
        {
            try
            {
                this._inputPlayer.Start();
                this._syncContext.Post(_ => State = PlaybackState.Completed, null);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Воспроизведение было остановлено (cancellationTokenSource.Cancel())"); // DEBUG
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex); // DEBUG
                this.State = PlaybackState.Stopped;
            }
        }

    }
}
