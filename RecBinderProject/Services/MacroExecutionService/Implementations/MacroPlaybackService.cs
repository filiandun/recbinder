using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Services.MacroExecutionService.Helpers;
using RecBinderProject.Services.MacroExecutionService.Interfaces;


namespace RecBinderProject.Services.MacroExecutionService.Implementations
{
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService
    {
        private readonly SynchronizationContext _syncContext;

        private Thread _playbackThread;

        private readonly MacroPlayer _player;
        private readonly Macro _macro; // TODO имеет смысл хранить не саму запись, а только события из неё (так как метаданные тут не используются)

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._player.GetElapsedTime();

        public MacroPlaybackService(MacroOptions macroOptions, Macro macro)
        {
            this._syncContext = SynchronizationContext.Current ?? throw new NullReferenceException("SynchronizationContext is null"); // TODO null

            this._playbackThread = new Thread(PlaybackWorker); // TODO заглушка (смысла в этой инициализации 0, только чтобы не ругалось на отсутствие инициализации в конструкторе)

            this._macro = macro;
            this._player = new MacroPlayer(this._macro, macroOptions);

            this.State = MacroExecutionState.Completed;
        }

        public void Start()
        {
            if (this.CanStart()) 
            {
                this.SetState(MacroExecutionState.Playing);

                this._playbackThread = new Thread(PlaybackWorker)
                {
                    Name = "PlaybackThread",
                    IsBackground = true
                };

                this._playbackThread.Start();
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this.SetState(MacroExecutionState.Paused);
                this._player.Pause();
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this.SetState(MacroExecutionState.Playing);
                this._player.Resume();
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this.SetState(MacroExecutionState.Stopped);
                this._player.Stop();
            }
        }


        private void PlaybackWorker()
        {
            try
            {
                this._player.Start();
                this._syncContext.Post(_ => State = MacroExecutionState.Completed, null);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Воспроизведение было остановлено (cancellationTokenSource.Cancel())"); // DEBUG
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex); // DEBUG
                this.SetState(MacroExecutionState.Stopped);
            }
        }
    }
}
