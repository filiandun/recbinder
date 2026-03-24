using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;
using RecBinderProject.Services.MacroExecutionService.Helpers;
using RecBinderProject.Services.MacroExecutionService.Interfaces;


namespace RecBinderProject.Services.MacroExecutionService.Implementations
{
    public class MacroRecordingService : MacroExecutionServiceBase, IMacroRecordingService
    {
        private CancellationTokenSource _cancellationToken;

        private readonly IMacroRepository _repository;

        private readonly MacroRecorder _recorder;
        private readonly Macro _macro;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._recorder.GetElapsedTime();

        public MacroRecordingService(IMacroRepository repository, MacroMetadata macroMetadata, MacroOptions macroOptions)
        {
            this._cancellationToken = new CancellationTokenSource();

            this._repository = repository;

            this._macro = new Macro(macroMetadata);
            this._recorder = new MacroRecorder(this._macro, macroOptions);

            this.State = MacroExecutionState.Completed;
        }


        public void Start(MacroMetadata macroMetadata)
        {
            if (this.CanStart())
            {
                this.SetState(MacroExecutionState.Playing);

                this._macro.UpdateMetadata(macroMetadata);

                this._cancellationToken = new CancellationTokenSource();

                this._recorder.Start();

                _ = WaitAndStopAfterDurationAsync(this._cancellationToken.Token);
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this.SetState(MacroExecutionState.Paused);
                this._recorder.Pause();
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this.SetState(MacroExecutionState.Playing);
                this._recorder.Resume();
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._recorder.Stop();
                this._cancellationToken.Cancel();

                this.SaveMacro();

                this.SetState(MacroExecutionState.Completed);
            }
        }


        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._macro.Metadata.Duration, token);
                this.Stop();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Досрочная ручная остановка записи");
            }
        }

        private void SaveMacro()
        {
            this._macro.Metadata.Duration = this._recorder.GetElapsedTime();
            this._macro.Metadata.CreationDateTime = DateTime.Now;

            Debug.WriteLine($"RecordingService: {this._macro.Events.Count}");

            this._repository.Save(this._macro);
        }
    }
}
