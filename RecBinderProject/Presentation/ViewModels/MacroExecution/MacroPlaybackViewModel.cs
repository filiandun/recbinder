using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Presentation.Services;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroOptions;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroPlaybackViewModel : MacroExecutionViewModelBase
    {
        private readonly IMacroPlaybackService _playbackService;

        public MacroMetadataViewerViewModel MetadataViewerViewModel { get; private set; }
        public MacroPlaybackOptionsViewModel OptionsViewModel { get; private set; }


        public MacroPlaybackViewModel(
            ILogger<MacroPlaybackViewModel> logger,

            IDialogService dialogService,

            IMacroPlaybackService playbackService,
            LocalizationViewModel localizationViewModel, 

            MacroPlaybackStatusViewModel statusViewModel, 
            MacroMetadataViewerViewModel metadataViewerViewModel,
            MacroPlaybackOptionsViewModel optionsViewModel) : base(logger, dialogService, playbackService, localizationViewModel, statusViewModel)
        {
            this._playbackService = playbackService;

            this.MetadataViewerViewModel = metadataViewerViewModel;
            this.OptionsViewModel = optionsViewModel;
        }


        protected override async void OnStartCommand()
        {
            try
            {
                if (this._executionService.State == MacroExecutionState.Paused)
                {
                    this._playbackService.Resume();
                }
                else
                {
                    await this._playbackService.StartAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, "An error occurred while playing the macro.");

                this._dialogService.Show(this.LocalizationViewModel.Common["Dialog_Label_Error"], "An error occurred while playing the macro.");
            }
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
