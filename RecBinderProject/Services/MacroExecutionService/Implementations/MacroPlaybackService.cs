using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Services.MacroExecutionService.Helpers;
using RecBinderProject.Services.MacroExecutionService.Interfaces;


namespace RecBinderProject.Services.MacroExecutionService.Implementations
{
    // TODO после ручной остановки запись больше нормально не воспроизводится
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly Macro _macro; // TODO имеет смысл хранить не саму запись, а только события из неё (так как метаданные тут не используются)
        private readonly MacroPlayer _player;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._player.GetElapsedTime();


        public MacroPlaybackService(MacroOptions macroOptions, Macro macro)
        {
            this._cancellationTokenSource = new CancellationTokenSource();

            this._macro = macro;
            this._player = new MacroPlayer(this._macro, macroOptions);

            this.State = MacroExecutionState.Completed;
        }


        public async Task Start()
        {
            if (this.CanStart())
            {
                this.SetState(MacroExecutionState.Playing);

                try
                {
                    this._cancellationTokenSource = new CancellationTokenSource();
                    //await _player.Start(this._cancellationTokenSource.Token);
                    await Task.Run(() => this._player.Start(this._cancellationTokenSource.Token));

                    this.SetState(MacroExecutionState.Completed);
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Воспроизведение было остановлено (cancellationTokenSource.Cancel())"); // DEBUG
                    this.SetState(MacroExecutionState.Stopped);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex); // DEBUG
                    this.SetState(MacroExecutionState.Stopped);
                }
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._player.Pause();
                this.SetState(MacroExecutionState.Paused);
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._player.Resume();
                this.SetState(MacroExecutionState.Playing);
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._cancellationTokenSource.Cancel();
                this._player.Stop();

                this.SetState(MacroExecutionState.Stopped);
            }
        }
    }
}
