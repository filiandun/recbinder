using System.Diagnostics;

using RecBinderProject.Models;


namespace RecBinderProject.Services.RecordService
{
    public class MacroPlaybackService : BaseRecordService
    {
        private readonly SynchronizationContext _syncContext;

        private Thread _playbackThread;

        private readonly Macro _macro; // TODO имеет смысл хранить не саму запись, а только события из неё (так как метаданные тут не используются)
        private readonly MacroPlayer _macroPlayer;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._macroPlayer.GetElapsedTime();

        public override MacroMetadata MacroMetadata => this._macro.Metadata;

        public MacroPlaybackService(MacroOptions macroOptions, Macro macro) : base(macroOptions)
        {
            this._syncContext = SynchronizationContext.Current ?? throw new NullReferenceException("SynchronizationContext is null"); // TODO null

            this._playbackThread = new Thread(PlaybackWorker); // TODO заглушка (смысла в этой инициализации 0, только чтобы не ругалось на отсутствие инициализации в конструкторе)

            this._macro = macro;
            this._macroPlayer = new MacroPlayer(this._macro, macroOptions);
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
                this._macroPlayer.Pause();
            }
        }

        public override void Resume()
        {
            if (this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Playing;
                this._macroPlayer.Resume();
            }
        }

        public override void Stop()
        {
            if (this.State == PlaybackState.Playing || this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Stopped;
                this._macroPlayer.Stop();
            }
        }

        private void PlaybackWorker()
        {
            try
            {
                this._macroPlayer.Start();
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
