using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Domain;

using RecBinderProject.Presentation.Services.Dialog;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroOptions;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroPlaybackViewModel : MacroExecutionViewModelBase
    {
        private readonly IMacroPlaybackService _playbackService;
        private readonly Macro _macro;

        public MacroMetadataViewerViewModel MetadataViewerViewModel { get; private set; }
        public MacroPlaybackOptionsViewModel OptionsViewModel { get; private set; }


        public MacroPlaybackViewModel(
            ILogger<MacroPlaybackViewModel> logger,

            IDialogService dialogService,

            IMacroPlaybackService playbackService,
            Macro macro,

            LocalizationViewModel localizationViewModel, 

            MacroPlaybackStatusViewModel statusViewModel) : base(logger, dialogService, playbackService, localizationViewModel, statusViewModel)
        {
            this._playbackService = playbackService;
            this._macro = macro;

            this.MetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);
            this.OptionsViewModel = new MacroPlaybackOptionsViewModel();

        }


        protected override async void OnStartCommand()
        {
            this._logger.LogInformation("Start/Resume command playback a macro '{MacroName}'.", this.MetadataViewerViewModel.MacroName);

            try
            {
                if (this._executionService.State == MacroExecutionState.Paused)
                {
                    this._logger.LogDebug("Resume playback a macro '{MacroName}'.", this.MetadataViewerViewModel.MacroName);

                    this._playbackService.Resume();
                }
                else
                {
                    this._logger.LogDebug("Start playback a macro '{MacroName}'.", this.MetadataViewerViewModel.MacroName);

                    await this._playbackService.StartAsync(this._macro, this.OptionsViewModel.Options);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while playing the macro.");

                await this._dialogService.ShowAsync(this.LocalizationViewModel.Common["Dialog_Label_Error"], "An error occurred while playing the macro.");
            }
        }

        protected override void OnPauseCommand()
        {
            this._logger.LogInformation("Pause command playback a macro '{MacroName}'.", this.MetadataViewerViewModel.MacroName);

            this._playbackService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._logger.LogInformation("Stop command playback a macro '{MacroName}'.", this.MetadataViewerViewModel.MacroName);

            this._playbackService.Stop();
        }
    }
}
