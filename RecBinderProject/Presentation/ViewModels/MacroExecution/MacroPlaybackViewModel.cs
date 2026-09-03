using System.Diagnostics;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroOptions;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroPlaybackViewModel : MacroExecutionViewModelBase
    {
        private readonly IMacroPlaybackService _playbackService;

        public MacroMetadataViewerViewModel MetadataViewerViewModel { get; private set; }
        public MacroPlaybackOptionsViewModel OptionsViewModel { get; private set; }


        public MacroPlaybackViewModel(
            IMacroPlaybackService playbackService,
            LocalizationViewModel localizationViewModel, 
            MacroPlaybackStatusViewModel statusViewModel, 
            MacroMetadataViewerViewModel metadataViewerViewModel,
            MacroPlaybackOptionsViewModel optionsViewModel) : base(playbackService, localizationViewModel, statusViewModel)
        {
            this._playbackService = playbackService;

            this.MetadataViewerViewModel = metadataViewerViewModel;
            this.OptionsViewModel = optionsViewModel;
        }


        protected override void OnStartCommand()
        {
            if (this._service.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
            {
                this._playbackService.Resume();
            }
            else
            {
                this._playbackService.Start();
            }
        }

        protected override void OnResumeCommand()
        {
            //this._recordProcess.Start();
            Debug.WriteLine("Play resume TODO");
        }

        protected override void OnPauseCommand()
        {
            this._playbackService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._playbackService.Stop();
        }
    }
}
