using System.Diagnostics;

using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly Macro _macro;
        private readonly MacroEventPlayer _player;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._player.GetElapsedTime();


        public MacroPlaybackService(MacroPlaybackOptions options, Macro macro)
        {
            this._cancellationTokenSource = new CancellationTokenSource();

            this._macro = macro;
            this._player = new MacroEventPlayer(this._macro, options);

            this.State = MacroExecutionState.Completed;
        }


        public async Task Start()
        {
            if (this.CanStart())
            {
                this.State = MacroExecutionState.Playing;

                try
                {
                    this._cancellationTokenSource.Dispose();
                    this._cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(() => this._player.Start(this._cancellationTokenSource.Token));

                    this.State = MacroExecutionState.Completed;
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Воспроизведение было остановлено (cancellationTokenSource.Cancel())"); // DEBUG
                    this.State = MacroExecutionState.Stopped;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex); // DEBUG
                    this.State = MacroExecutionState.Stopped;
                }
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._player.Pause();

                this.State = MacroExecutionState.Paused;
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._player.Resume();

                this.State = MacroExecutionState.Playing;
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._cancellationTokenSource.Cancel();

                this.State = MacroExecutionState.Stopped;
            }
        }
    }
}
