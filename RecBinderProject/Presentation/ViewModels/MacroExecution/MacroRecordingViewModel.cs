using System.Diagnostics;

using ModernWpf.Controls;

using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;
using RecBinderProject.Presentation.ViewModels.Localization;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroRecordingViewModel : MacroExecutionViewModelBase
    {
        public event Action? MacroRecorded;

        private readonly IMacroRecordingService _macroRecordingService;

        public MacroMetadataCreatorViewModel MacroMetadataCreatorViewModel { get; init; }


        public MacroRecordingViewModel(
            IMacroRecordingService macroRecordingService,
            LocalizationViewModel localizationViewModel, 
            MacroMetadataCreatorViewModel macroMetadataCreatorViewModel, 
            MacroExecutionStatusViewModel macroExecutionStatusViewModel,
            MacroExecutionOptions macroOptions) : base(macroRecordingService, localizationViewModel, macroExecutionStatusViewModel, macroOptions)
        {
            this._macroRecordingService = macroRecordingService;

            this.MacroMetadataCreatorViewModel = macroMetadataCreatorViewModel;

            this._macroRecordingService.MacroSaved += this.MacroRecordingService_MacroSaved;
        }

        private void MacroRecordingService_MacroSaved()
        {
            Debug.WriteLine("MACRO SAVED");
            this.MacroRecorded?.Invoke();
        }

        protected override void OnStartCommand()
        {
            try
            {
                if (this._macroRecordingService.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
                {
                    this._macroRecordingService.Resume();
                }
                else
                {
                    this.MacroMetadataCreatorViewModel.ApplyChanges(); // TODO сделать адекватнее как-нибудь

                    this._macroRecordingService.Start(this.MacroMetadataCreatorViewModel.MacroMetadataModel);
                }
            }
            catch (ArgumentException ae)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ae.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ex.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
        }

        protected override void OnResumeCommand() // TODO под вопросом, так как отдельной кнопки продолжить нет (реализуется всё через кнопку start)
        {
            Debug.WriteLine("Play resume TODO");
        }

        protected override void OnPauseCommand()
        {
            this._macroRecordingService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._macroRecordingService.Stop();
        }
    }
}

