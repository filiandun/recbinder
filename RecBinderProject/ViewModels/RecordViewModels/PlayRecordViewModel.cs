using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Services.RecordService;
using RecBinderProject.ViewModels.RecordingMetadataViewModels;


namespace RecBinderProject.ViewModels.RecordViewModels
{
    public class PlayRecordViewModel : BaseRecordViewModel
    {
        public PlayRecordViewModel(IRecordProcess recordProcess, RecordingInput recordingInput, ILocalizationService localizationService, string localizationPrefix) : base(recordProcess, localizationService, localizationPrefix)
        {
            this._recordProcess = recordProcess;

            this._recordingMetadataViewModel = new RecordingMetadataViewModel(recordingInput.RecordingMetadata);
        }

        protected override void Start()
        {
            if (this._recordProcess.State == PlaybackState.Paused)
            {
                this._recordProcess.Resume();
            }
            else
            {
                this._recordProcess.Start();
            }
        }

        protected override void Resume()
        {
            //this._recordProcess.Start();
            Debug.WriteLine("Play resume TODO");
        }

        protected override void Pause()
        {
            this._recordProcess.Pause();
        }

        protected override void Stop()
        {
            this._recordProcess.Stop();
        }
    }
}
