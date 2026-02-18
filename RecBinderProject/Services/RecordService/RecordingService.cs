using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Models.RecordingRepository;


namespace RecBinderProject.Services.RecordService
{
    public class RecordingService : BaseRecordService
    {
        private CancellationTokenSource _recordingCancellationToken;

        private readonly IRecordingRepository _recordingRepository;

        private RecordingInput _recordingInput;
        private readonly InputRecorder _inputRecorder;

        public override TimeSpan TotalTime => this._recordingInput.RecordingMetadata.Duration;
        public override TimeSpan ElapsedTime => this._inputRecorder.GetElapsedTime();

        public override RecordingMetadata RecordingMetadata => this._recordingInput.RecordingMetadata;

        //public override RecordingInput RecordingInput => this._recordingInput;

        public RecordingService(IRecordingRepository recordingRepository, RecordingMetadata recordingMetadata, RecordOptions recordOptions) : base(recordOptions)
        {
            this._recordingCancellationToken = new CancellationTokenSource();

            this._recordingRepository = recordingRepository;

            this._recordingInput = new RecordingInput(recordingMetadata);
            this._inputRecorder = new InputRecorder(this._recordingInput, recordOptions);
        }

        // TODO немного ебаный метод
        public void UpdateMetadata(RecordingMetadata recordingMetadata)
        {
            this._recordingInput.RecordingMetadata.Name = recordingMetadata.Name;
            this._recordingInput.RecordingMetadata.Description = recordingMetadata.Description;
            this._recordingInput.RecordingMetadata.Duration = recordingMetadata.Duration;
            this._recordingInput.RecordingMetadata.CreationDateTime = recordingMetadata.CreationDateTime;
        }

        public override void Start()
        {
            this._recordingCancellationToken = new CancellationTokenSource();

            this.State = PlaybackState.Playing;
            this._inputRecorder.Start();

            _ = WaitAndStopAfterDurationAsync(this._recordingCancellationToken.Token);
        }

        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._recordingInput.RecordingMetadata.Duration, token);
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
                this._inputRecorder.Pause();
            }
        }

        public override void Resume()
        {
            if (this.State == PlaybackState.Paused)
            {
                this.State = PlaybackState.Playing;
                this._inputRecorder.Resume();
            }
        }

        public override void Stop()
        {
            if (this.State == PlaybackState.Playing || this.State == PlaybackState.Paused)
            {
                Debug.WriteLine("Recording state completed");
                this.State = PlaybackState.Completed;

                this._inputRecorder.Stop();
                this._recordingCancellationToken.Cancel();

                this.SaveRecordingInput();
            }
        }

        private void SaveRecordingInput()
        {
            this._recordingInput.RecordingMetadata.Duration = this._inputRecorder.GetElapsedTime();
            this._recordingInput.RecordingMetadata.CreationDateTime = DateTime.Now;

            Debug.WriteLine($"RecordingService: {this._recordingInput.InputEvents.InputEventsList.Count}");

            this._recordingRepository.Save(this._recordingInput);
        }
    }
}
