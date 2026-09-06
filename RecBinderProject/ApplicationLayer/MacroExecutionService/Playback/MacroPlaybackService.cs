using Microsoft.Extensions.Logging;

using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService
    {
        private readonly ILogger<MacroPlaybackService> _logger;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Macro _macro;
        private readonly MacroEventPlayer _player;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._player.GetElapsedTime();


        public MacroPlaybackService(ILoggerFactory loggerFactory, Macro macro, MacroPlaybackOptions options)
        {
            this._logger = loggerFactory.CreateLogger<MacroPlaybackService>();

            this._cancellationTokenSource = new CancellationTokenSource();

            this._macro = macro;

            var playerLogger = loggerFactory.CreateLogger<MacroEventPlayer>();
            this._player = new MacroEventPlayer(playerLogger, this._macro, options);

            this.State = MacroExecutionState.Completed;
        }


        public async Task Start()
        {
            if (this.CanStart())
            {
                this._logger.LogInformation("Starting macro playback, {name}", this._macro.Metadata.Name);

                this.State = MacroExecutionState.Playing;

                try
                {
                    this._cancellationTokenSource.Dispose();
                    this._cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(() => this._player.Start(this._cancellationTokenSource.Token));

                    this._logger.LogInformation("Macro playback completed successfully.");

                    this.State = MacroExecutionState.Completed;
                }
                catch (OperationCanceledException)
                {
                    this._logger.LogInformation("Macro playback was stopped by user.");

                    this.State = MacroExecutionState.Stopped;
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Macro playback terminated with an unexpected error.");

                    this.State = MacroExecutionState.Stopped;
                }
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._logger.LogInformation("Macro playback paused.");

                this._player.Pause();

                this.State = MacroExecutionState.Paused;
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._logger.LogInformation("Macro playback resumed.");

                this._player.Resume();

                this.State = MacroExecutionState.Playing;
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._logger.LogInformation("Macro playback stopping..");

                this._cancellationTokenSource.Cancel();
            }
        }
    }
}
