using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;

using RecBinderProject.Presentation.Services;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroOptions;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroRecordingViewModel : MacroExecutionViewModelBase
    {
        private readonly IMacroRecordingService _recordingService;

        public MacroMetadataCreatorViewModel MetadataCreatorViewModel { get; private set; }
        public MacroRecordingOptionsViewModel OptionsViewModel { get; private set; }


        public MacroRecordingViewModel(
            ILogger<MacroRecordingViewModel> logger,

            IDialogService dialogService,

            IMacroRecordingService recordingService,
            LocalizationViewModel localizationViewModel, 

            MacroRecordingStatusViewModel statusViewModel,
            MacroMetadataCreatorViewModel metadataCreatorViewModel,
            MacroRecordingOptionsViewModel optionsViewModel) : base(logger, dialogService, recordingService, localizationViewModel, statusViewModel)
        {
            this._recordingService = recordingService;

            this.MetadataCreatorViewModel = metadataCreatorViewModel;
            this.OptionsViewModel = optionsViewModel;
        }


        protected override async void OnStartCommand()
        {
            try
            {
                if (this._recordingService.State == MacroExecutionState.Paused)
                {
                    this._recordingService.Resume();
                }
                else
                {
                    this.MetadataCreatorViewModel.ApplyChanges(); // TODO сделать адекватнее как-нибудь

                    await this._recordingService.Start(this.MetadataCreatorViewModel.MacroMetadataModel);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, "An error occurred while recording the macro.");

                this._dialogService.Show(this.LocalizationViewModel.Common["Dialog_Label_Error"], "An error occurred while recording the macro.");
            }
        }

        protected override void OnPauseCommand()
        {
            this._recordingService.Pause();
        }

        protected override void OnStopCommand()
        {
            try
            {
                this._recordingService.Stop();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, "An error occurred while saving the macro.");

                this._dialogService.Show(this.LocalizationViewModel.Common["Dialog_Label_Error"], "An error occurred while saving the macro.");
            }
        }
    }
}

