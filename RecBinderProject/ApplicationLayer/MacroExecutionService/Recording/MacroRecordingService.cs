using System.Diagnostics;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.Repositories;
using RecBinderProject.Infrastructure.Services.Input;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Recording
{
    public class MacroRecordingService : MacroExecutionServiceBase, IMacroRecordingService
    {
        public event Action? MacroSaved;

        private readonly ILogger<MacroRecordingService> _logger;
        private readonly ILogger<MacroEventRecorder> _recorderLogger;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly IMacroRepository _repository;

        private readonly IInputListener _inputListener;

        private Macro? _macro;
        private MacroEventRecorder? _recorder;

        private readonly MacroRecordingOptions _options;

        public override TimeSpan TotalDuration => this._macro?.Metadata.Duration ?? TimeSpan.Zero;
        public override TimeSpan ElapsedTime => this._recorder?.GetElapsedTime() ?? TimeSpan.Zero;


        public MacroRecordingService(ILogger<MacroRecordingService> logger, ILogger<MacroEventRecorder> recorderLogger, IMacroRepository repository, IInputListener inputListener, MacroRecordingOptions macroOptions)
        {
            this._logger = logger;
            this._recorderLogger = recorderLogger;

            this._cancellationTokenSource = new CancellationTokenSource();

            this._repository = repository;

            this._inputListener = inputListener;

            this._macro = null;
            this._recorder = null;

            this._options = macroOptions;

            this.State = MacroExecutionState.Completed;
        }


        public async Task Start(MacroMetadata macroMetadata)
        {
            if (this.CanStart())
            {
                this._macro = new Macro(macroMetadata);
                this._recorder = new MacroEventRecorder(this._recorderLogger, this._inputListener, this._macro, this._options);

                this._cancellationTokenSource.Dispose();
                this._cancellationTokenSource = new CancellationTokenSource();

                this._logger.LogInformation("Starting macro recording, {name}", this._macro.Metadata.Name);

                this._recorder.Start();

                this.State = MacroExecutionState.Playing;

                await this.WaitAndStopAfterDurationAsync(this._cancellationTokenSource.Token);
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._logger.LogInformation("Macro recording paused.");

                this._recorder!.Pause();

                this.State = MacroExecutionState.Paused;
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._logger.LogInformation("Macro recording resumed.");

                this._recorder!.Resume();

                this.State = MacroExecutionState.Playing;
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._logger.LogInformation("Macro recording stopped.");

                this._recorder!.Stop();
                this._cancellationTokenSource.Cancel();

                this.State = MacroExecutionState.Stopped;
            }
        }


        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._macro!.Metadata.Duration, token);

                this._logger.LogInformation("Macro recording completed.");

                this._recorder!.Stop();

                this.State = MacroExecutionState.Completed;
            }
            catch (OperationCanceledException)
            {
                this._logger.LogInformation("Macro recording was stopped by user.");
            }
        }

        public async Task SaveMacroAsync()
        {
            if (this._macro is null)
            {
                this._logger.LogWarning("Attempt to save a null macro.");
                return;
            }

            if (this._recorder is null)
            {
                this._logger.LogWarning("Attempt to save a macro when recorder is null.");
                return;
            }

            this._macro.Metadata.Duration = this._recorder.GetElapsedTime();
            this._macro.Metadata.CreationDateTime = DateTime.Now;

            await this._repository.SaveAsync(this._macro);

            this._logger.LogInformation("Save macro with '{EventCount}' events.", this._macro.Events.Count);
        }
    }
}
