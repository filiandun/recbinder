using System.Diagnostics;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;

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
            IMacroRecordingService recordingService,
            LocalizationViewModel localizationViewModel, 
            MacroRecordingStatusViewModel statusViewModel,
            MacroMetadataCreatorViewModel metadataCreatorViewModel,
            MacroRecordingOptionsViewModel optionsViewModel) : base(recordingService, localizationViewModel, statusViewModel)
        {
            this._recordingService = recordingService;

            this.MetadataCreatorViewModel = metadataCreatorViewModel;
            this.OptionsViewModel = optionsViewModel;
        }


        protected override void OnStartCommand()
        {
            //try
            //{
            if (this._recordingService.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
            {
                this._recordingService.Resume();
            }
            else
            {
                this.MetadataCreatorViewModel.ApplyChanges(); // TODO сделать адекватнее как-нибудь

                this._recordingService.Start(this.MetadataCreatorViewModel.MacroMetadataModel);
            }
            //}
            //catch (ArgumentException ae)
            //{
            //    //ContentDialog contentDialog = new ContentDialog()
            //    //{
            //    //    Content = ae.Message,
            //    //    CloseButtonText = "Закрыть",
            //    //};
            //    //contentDialog.ShowAsync();
            //}
            //catch (Exception ex)
            //{
            //    //ContentDialog contentDialog = new ContentDialog()
            //    //{
            //    //    Content = ex.Message,
            //    //    CloseButtonText = "Закрыть",
            //    //};
            //    //contentDialog.ShowAsync();
            //}
        }

        protected override void OnResumeCommand() // TODO под вопросом, так как отдельной кнопки продолжить нет (реализуется всё через кнопку start)
        {
            Debug.WriteLine("Play resume TODO");
        }

        protected override void OnPauseCommand()
        {
            this._recordingService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._recordingService.Stop();
        }
    }
}

