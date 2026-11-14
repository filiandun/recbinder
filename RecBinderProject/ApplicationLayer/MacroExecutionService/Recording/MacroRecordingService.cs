using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Managers;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.Repositories;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Recording
{
    public class MacroRecordingService : MacroExecutionServiceBase, IMacroRecordingService, IDisposable
    {
        private readonly ILogger<MacroRecordingService> _logger;

        private readonly IMacroEventRecorderFactory _recorderFactory;
        private readonly IMacroRepository _repository;

        private MacroRecordingSession? _recordingSession;
        private MacroRecordingSession ActiveRecordingSession => this._recordingSession ?? throw new InvalidOperationException("Recording session is not initialized.");

        public override TimeSpan TotalDuration => this._recordingSession?.Macro.Metadata.Duration ?? TimeSpan.Zero;
        public override TimeSpan ElapsedTime => this._recordingSession?.MacroRecorder.GetElapsedTime() ?? TimeSpan.Zero;

        public bool CanSave => this.State == MacroExecutionState.Completed || this.State == MacroExecutionState.Stopped;


        public MacroRecordingService(ILogger<MacroRecordingService> logger, IMacroEventRecorderFactory recorderFactory, IMacroRepository repository)
        {
            this._logger = logger;

            this._recorderFactory = recorderFactory;
            this._repository = repository;
            
            this._recordingSession = null;
        }


        public async Task StartAsync(MacroMetadata macroMetadata, MacroRecordingOptions recordingOptions)
        {
            if (!this.CanStart)
            {
                this._logger.LogWarning("Attempt start session while state is '{State}'.", this.State);

                return;
            }

            this._logger.LogInformation("Starting macro recording, {name}", macroMetadata.Name);

            this.SetState(MacroExecutionState.Executing);

            this._recordingSession?.Dispose();
            this._recordingSession = null;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                Macro macro = new Macro(macroMetadata);

                MacroEventRecorder macroRecorder = this._recorderFactory.Create(recordingOptions, macro);

                this._recordingSession = new MacroRecordingSession(macroRecorder, cancellationTokenSource, recordingOptions, macro);

                macroRecorder.Start();

                await this.WaitAndStopAfterDurationAsync(this._recordingSession);

                this._logger.LogInformation("Recording macro '{Name}' completed successfully.", macro.Metadata.Name);
            }
            catch
            {
                cancellationTokenSource?.Dispose();

                this._recordingSession?.Dispose();
                this._recordingSession = null;

                this.SetState(MacroExecutionState.Stopped); // TODO MacroExecutionState.Faulted

                throw;
            }
        }

        public void Pause()
        {
            if (this.CanPause)
            {
                this._logger.LogInformation("Macro recording paused.");

                this.ActiveRecordingSession.MacroRecorder.Pause();

                this.SetState(MacroExecutionState.Paused);
            }
        }

        public void Resume()
        {
            if (this.CanResume)
            {
                this._logger.LogInformation("Macro recording resumed.");

                this.ActiveRecordingSession.MacroRecorder.Resume();

                this.SetState(MacroExecutionState.Executing);
            }
        }

        public void Stop()
        {
            if (this.CanStop)
            {
                this._logger.LogInformation("Macro recording stopped.");

                this.ActiveRecordingSession.MacroRecorder.Stop();
                this.ActiveRecordingSession.CancellationTokenSource.Cancel();

                this.SetState(MacroExecutionState.Stopped);
            }
        }


        private async Task WaitAndStopAfterDurationAsync(MacroRecordingSession recordingSession)
        {
            try
            {
                await Task.Delay(recordingSession.Macro.Metadata.Duration, recordingSession.CancellationTokenSource.Token);

                this._logger.LogInformation("Macro recording completed.");

                recordingSession.MacroRecorder.Stop();

                this.SetState(MacroExecutionState.Completed);
            }
            catch (OperationCanceledException)
            {
                this._logger.LogInformation("Macro recording was stopped by user.");
            }
        }

        public async Task SaveMacroAsync()
        {
            if (this.CanSave)
            {
                this.ActiveRecordingSession.Macro.Metadata.Duration = this.ElapsedTime;
                this.ActiveRecordingSession.Macro.Metadata.CreationDateTime = DateTime.Now;

                await this._repository.SaveAsync(this.ActiveRecordingSession.Macro);

                this._logger.LogInformation("Save macro with '{EventCount}' events.", this.ActiveRecordingSession.Macro.Events.Count);
            }
        }


        public void Dispose()
        {
            this._recordingSession?.Dispose();
        }
    }
}
