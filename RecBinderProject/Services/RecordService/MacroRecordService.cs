using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;

namespace RecBinderProject.Services.RecordService
{
    public class MacroRecordService : BaseRecordService
    {
        private CancellationTokenSource _cancellationToken;

        private readonly IMacroRepository _repository;

        private Macro _macro;
        private readonly MacroRecorder _recorder;

        public override TimeSpan TotalTime => this._macro.Metadata.Duration;
        public override TimeSpan ElapsedTime => this._recorder.GetElapsedTime();

        public override MacroMetadata MacroMetadata => this._macro.Metadata;

        //public override RecordingInput RecordingInput => this._recordingInput;

        public MacroRecordService(IMacroRepository repository, MacroMetadata macroMetadata, MacroOptions macroOptions) : base(macroOptions)
        {
            this._cancellationToken = new CancellationTokenSource();

            this._repository = repository;

            this._macro = new Macro(macroMetadata);
            this._recorder = new MacroRecorder(this._macro, macroOptions);
        }

        // TODO немного ебаный метод
        public void UpdateMetadata(MacroMetadata recordingMetadata)
        {
            this._macro.Metadata.Name = recordingMetadata.Name;
            this._macro.Metadata.Description = recordingMetadata.Description;
            this._macro.Metadata.Duration = recordingMetadata.Duration;
            this._macro.Metadata.CreationDateTime = recordingMetadata.CreationDateTime;
        }

        public override void Start()
        {
            this._cancellationToken = new CancellationTokenSource();

            this.State = PlaybackState.Playing;
            this._recorder.Start();

            _ = WaitAndStopAfterDurationAsync(this._cancellationToken.Token);
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

        public override void Pause()
        {
            if (this.State == PlaybackState.Playing)
            {
                this.State = PlaybackState.Paused;
                this._recorder.Pause();
            }
        }

        public override void Resume()
        {
            if (this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Playing;
                this._recorder.Resume();
            }
        }

        public override void Stop()
        {
            if (this.State == PlaybackState.Playing || this.State == PlaybackState.Paused)
            {
                Debug.WriteLine("Recording state completed");
                this.State = PlaybackState.Completed;

                this._recorder.Stop();
                this._cancellationToken.Cancel();

                this.SaveRecordingInput();
            }
        }

        private void SaveRecordingInput()
        {
            this._macro.Metadata.Duration = this._recorder.GetElapsedTime();
            this._macro.Metadata.CreationDateTime = DateTime.Now;

            Debug.WriteLine($"RecordingService: {this._macro.Events.Count}");

            this._repository.Save(this._macro);
        }
    }
}
