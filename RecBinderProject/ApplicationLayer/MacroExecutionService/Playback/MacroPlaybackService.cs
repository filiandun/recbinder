using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Managers;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public class MacroPlaybackService : MacroExecutionServiceBase, IMacroPlaybackService, IDisposable
    {
        private readonly ILogger<MacroPlaybackService> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        private MacroPlaybackSession? _playbackSession;
        private MacroPlaybackSession ActivePlaybackSession => this._playbackSession ?? throw new InvalidOperationException("Playback session is not initialized.");

        public override TimeSpan TotalDuration => this._playbackSession?.MacroPlayer.EffectiveDuration ?? TimeSpan.Zero;
        public override TimeSpan ElapsedTime => this._playbackSession?.MacroPlayer.ElapsedTime ?? TimeSpan.Zero;


        public MacroPlaybackService(ILogger<MacroPlaybackService> logger, IServiceScopeFactory scopeFactory)
        {
            this._logger = logger;

            this._scopeFactory = scopeFactory;

            this._playbackSession = null;
        }


        public async Task StartAsync(Macro macro, MacroPlaybackOptions playbackOptions)
        {
            if (!this.CanStart)
            {
                this._logger.LogWarning("Attempt start session while state is '{State}'.", this.State);

                return;
            }

            this._logger.LogInformation("Starting macro playback. Name: {Name}, Speed: {Speed}, Loop count: {LoopCount}", macro.Metadata.Name, playbackOptions.Speed, playbackOptions.LoopCount);

            this.SetState(MacroExecutionState.Executing);

            this._playbackSession?.Dispose();
            this._playbackSession = null;

            IServiceScope serviceScope = this._scopeFactory.CreateScope();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                MacroPlaybackContext playbackContext = serviceScope.ServiceProvider.GetRequiredService<MacroPlaybackContext>();
                playbackContext.Initialize(macro, playbackOptions);

                MacroEventPlayer macroPlayer = serviceScope.ServiceProvider.GetRequiredService<MacroEventPlayer>();

                this._playbackSession = new MacroPlaybackSession(serviceScope, macroPlayer, cancellationTokenSource, playbackOptions, macro);

                await Task.Run(async () => await this.PlaybackLoop(this._playbackSession));

                this._logger.LogInformation("Playback macro '{Name}' completed successfully.", macro.Metadata.Name);

                this.SetState(MacroExecutionState.Completed);
            }
            catch (OperationCanceledException)
            {
                this._logger.LogInformation("Playback macro '{Name}' was stopped by user.", macro.Metadata.Name);

                this.SetState(MacroExecutionState.Stopped);
            }
            catch
            {
                serviceScope?.Dispose();
                cancellationTokenSource?.Dispose();

                this.SetState(MacroExecutionState.Stopped); // TODO MacroExecutionState.Faulted

                throw;
            }
            finally
            {
                this._playbackSession?.Dispose();
                this._playbackSession = null;
            }
        }

        public void Pause()
        {
            if (this.CanPause)
            {
                this._logger.LogInformation("Macro playback paused.");

                this.ActivePlaybackSession.MacroPlayer.Pause();

                this.SetState(MacroExecutionState.Paused);
            }
        }

        public void Resume()
        {
            if (this.CanResume)
            {
                this._logger.LogInformation("Macro playback resumed.");

                this.ActivePlaybackSession.MacroPlayer.Resume();

                this.SetState(MacroExecutionState.Executing);
            }
        }

        public void Stop()
        {
            if (this.CanStop)
            {
                this._logger.LogInformation("Macro playback stopping..");

                this.ActivePlaybackSession.CancellationTokenSource.Cancel();
            }
        }


        private async Task PlaybackLoop(MacroPlaybackSession playbackSession)
        {
            int iteration = 0;

            while (!playbackSession.CancellationTokenSource.IsCancellationRequested)
            {
                this._logger.LogInformation("Continue macro playback. Iteration: {Iteration}/{LoopCount}", iteration, playbackSession.PlaybackOptions.LoopCount);

                iteration++;

                await playbackSession.MacroPlayer.Start(playbackSession.CancellationTokenSource.Token);

                if (!playbackSession.PlaybackOptions.LoopEnabled) break;

                if (playbackSession.PlaybackOptions.LoopCount > 0 && iteration >= playbackSession.PlaybackOptions.LoopCount) break;

                if (playbackSession.PlaybackOptions.LoopDelayMs > 0)
                {
                    await Task.Delay(playbackSession.PlaybackOptions.LoopDelayMs, playbackSession.CancellationTokenSource.Token);
                }
            }
        }


        public void Dispose()
        {
            this._playbackSession?.Dispose();
        }
    }
}
