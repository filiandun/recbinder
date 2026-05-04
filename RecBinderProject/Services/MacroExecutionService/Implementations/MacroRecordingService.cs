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

        private readonly Macro _macro;
        private readonly MacroRecorder _recorder;

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
                this._macro.UpdateMetadata(macroMetadata);

                this._cancellationToken = new CancellationTokenSource();

                this._recorder.Start();

                this.SetState(MacroExecutionState.Playing);

                _ = WaitAndStopAfterDurationAsync(this._cancellationToken.Token);
            }
        }

        public void Pause()
        {
            if (this.CanPause())
            {
                this._recorder.Pause();
                this.SetState(MacroExecutionState.Paused);
            }
        }

        public void Resume()
        {
            if (this.CanResume())
            {
                this._recorder.Resume();
                this.SetState(MacroExecutionState.Playing);
            }
        }

        public void Stop()
        {
            if (this.CanStop())
            {
                this._recorder.Stop();
                this._cancellationToken.Cancel();

                this.SaveMacro();

                this.SetState(MacroExecutionState.Stopped);
            }
        }


        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._macro.Metadata.Duration, token);

                this._recorder.Stop();
                this._cancellationToken.Cancel();

                this.SaveMacro();

                this.SetState(MacroExecutionState.Completed);
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
