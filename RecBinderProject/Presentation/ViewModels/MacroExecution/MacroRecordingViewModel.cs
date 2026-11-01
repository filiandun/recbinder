using System.IO;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;

using RecBinderProject.Presentation.Services.Dialog;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroOptions;



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
            this._logger.LogInformation("Start/Resume command recording a macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

            try
            {
                if (this._recordingService.State == MacroExecutionState.Paused)
                {
                    this._logger.LogDebug("Resume recording a macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                    this._recordingService.Resume();
                }
                else
                {
                    this._logger.LogDebug("Start recording a macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                    this.MetadataCreatorViewModel.ApplyChanges(); // TODO сделать адекватнее как-нибудь

                    await this._recordingService.Start(this.MetadataCreatorViewModel.MacroMetadataModel);

                    await this.TrySaveMacroAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, "An error occurred while recording the macro.");

                await this._dialogService.ShowAsync(this.LocalizationViewModel.Common["Dialog_Label_Error"], "An error occurred while recording the macro.");
            }
        }

        protected override void OnPauseCommand()
        {
            this._logger.LogInformation("Pause command recording a macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

            this._recordingService.Pause();
        }

        protected override async void OnStopCommand()
        {
            this._logger.LogInformation("Stop command recording a macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

            this._recordingService.Stop();
        }


        private async Task TrySaveMacroAsync()
        {
            string? errorMessage = null;

            try
            {
                await this._recordingService.SaveMacroAsync();

                this._logger.LogInformation("Successfully saved macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                return;
            }
            catch (UnauthorizedAccessException uaex)
            {
                this._logger.LogWarning(uaex, "No permissions to save the macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                errorMessage = "Access denied. Please check folder permissions or run as administrator.";
            }
            catch (IOException ioex)
            {
                this._logger.LogWarning(ioex, "I/O error while saving macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                if (ioex.HResult == unchecked((int)0x80070020))
                {
                    errorMessage = "The file is currently locked by another process. Please close any programs using it.";
                }
                else
                {
                    errorMessage = "Storage error. The disk might be full, write-protected, or disconnected.";
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Unexpected error saving macro '{MacroName}'.", this.MetadataCreatorViewModel.MacroName);

                errorMessage = $"An unexpected error occurred: {ex.Message}";
            }

            await this.ShowErrorDialog(errorMessage);
        }


        private async Task ShowErrorDialog(string errorMessage)
        {
            DialogResult dialogResult = await this._dialogService.ShowAsync
            (
                this.LocalizationViewModel.Common["Dialog_Label_Error"], 
                $"An error occurred while saving the macro\n{errorMessage}", 
                "Try again", 
                "Cancel save"
            );

            if (dialogResult == DialogResult.Primary)
            {
                this._logger.LogInformation("User chose to try again.");

                await this.TrySaveMacroAsync();
            }
            else
            {
                this._logger.LogInformation("User chose to cancel save.");
            }
        }
    }
}

