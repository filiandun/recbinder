using System.Diagnostics;

using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Infrastructure.Repositories;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Recording
{
    public class MacroRecordingService : MacroExecutionServiceBase, IMacroRecordingService
    {
        public event Action? MacroSaved;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly IMacroRepository _repository;

        private Macro? _macro;
        private MacroEventRecorder? _recorder;

        private readonly MacroRecordingOptions _options;

        public override TimeSpan TotalTime => this._macro?.Metadata.Duration ?? TimeSpan.Zero;
        public override TimeSpan ElapsedTime => this._recorder?.GetElapsedTime() ?? TimeSpan.Zero;


        public MacroRecordingService(IMacroRepository repository, MacroRecordingOptions macroOptions)
        {
            this._cancellationTokenSource = new CancellationTokenSource();

            this._repository = repository;

            this._options = macroOptions;

            this.State = MacroExecutionState.Completed;
        }


        public void Start(MacroMetadata macroMetadata)
        {
            if (this.CanStart())
            {
                this._macro = new Macro(macroMetadata);
                this._recorder = new MacroEventRecorder(this._macro, this._options);

                this._cancellationTokenSource = new CancellationTokenSource();

                this._recorder.Start();

                this.State = MacroExecutionState.Playing;

                _ = WaitAndStopAfterDurationAsync(this._cancellationTokenSource.Token);
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._recorder!.Pause();

                this.State = MacroExecutionState.Paused;
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._recorder!.Resume();

                this.State = MacroExecutionState.Playing;
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._recorder!.Stop();
                this._cancellationTokenSource.Cancel();

                this.SaveMacro();

                this.State = MacroExecutionState.Stopped;
            }
        }


        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._macro!.Metadata.Duration, token);

                this._recorder!.Stop();
                this._cancellationTokenSource.Cancel();

                this.SaveMacro();

                this.State = MacroExecutionState.Completed;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Досрочная ручная остановка записи");
            }
        }

        private void SaveMacro()
        {
            this._macro!.Metadata.Duration = this._recorder!.GetElapsedTime();
            this._macro.Metadata.CreationDateTime = DateTime.Now;

            Debug.WriteLine($"RecordingService: {this._macro.Events.Count}");

            this._repository.Save(this._macro);

            this.MacroSaved?.Invoke();
        }
    }
}
