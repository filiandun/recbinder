using System.Diagnostics;

using RecBinderProject.Models;
using RecBinderProject.Services.RecordService;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.ViewModels.MacroMetadataViewModels;


namespace RecBinderProject.ViewModels.RecordViewModels
{
    public class MacroPlaybackViewModel : MacroBindingViewModel
    {
        public MacroPlaybackViewModel(IRecordProcess recordProcess, Macro recordingInput, ILocalizationService localizationService, string localizationPrefix) : base(recordProcess, localizationService, localizationPrefix)
        {
            this._recordProcess = recordProcess;

            this._recordingMetadataViewModel = new MacroMetadataViewModel(recordingInput.Metadata);
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
