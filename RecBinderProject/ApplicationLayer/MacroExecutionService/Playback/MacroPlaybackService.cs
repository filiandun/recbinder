using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService
    {
        private readonly ILogger<MacroPlaybackService> _logger;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Macro _macro;
        private readonly MacroPlaybackOptions _options;

        private readonly MacroEventPlayer _player;

        public override TimeSpan TotalDuration => this._player.EffectiveDuration;
        public override TimeSpan ElapsedTime => this._player.ElapsedTime;


        public MacroPlaybackService(ILoggerFactory loggerFactory, Macro macro, MacroPlaybackOptions options)
        {
            this._logger = loggerFactory.CreateLogger<MacroPlaybackService>();

            this._cancellationTokenSource = new CancellationTokenSource();

            this._macro = macro;
            this._options = options;

            var playerLogger = loggerFactory.CreateLogger<MacroEventPlayer>();
            this._player = new MacroEventPlayer(playerLogger, this._macro, options);

            this.State = MacroExecutionState.Completed;
        }


        public async Task StartAsync()
        {
            if (this.CanStart())
            {
                this._logger.LogInformation("Starting macro playback. Name: {Name}, Speed: {Speed}, Loop count: {LoopCount}", this._macro.Metadata.Name, this._options.Speed, this._options.LoopCount);

                this.State = MacroExecutionState.Playing;

                try
                {
                    this._cancellationTokenSource.Dispose();
                    this._cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(async () => await this.PlaybackLoop());

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

                    throw;
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


        private async Task PlaybackLoop()
        {
            int iteration = 0;

            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                this._logger.LogInformation("Continue macro playback. Iteration: {Iteration}/{LoopCount}", iteration, this._options.LoopCount);

                iteration++;

                await this._player.Start(this._cancellationTokenSource.Token);

                if (!this._options.LoopEnabled) break;

                if (this._options.LoopCount > 0 && iteration >= this._options.LoopCount) break;

                if (this._options.LoopDelayMs > 0)
                {
                    await Task.Delay(this._options.LoopDelayMs, this._cancellationTokenSource.Token);
                }
            }
        }
    }
}
